﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moonglade.Auditing;
using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;
using Moonglade.Model;
using Moonglade.Model.Settings;

namespace Moonglade.Core
{
    public class MenuService : BlogService
    {
        private readonly IRepository<MenuEntity> _menuRepo;
        private readonly IBlogAudit _audit;

        public MenuService(
            ILogger<MenuService> logger,
            IOptions<AppSettings> settings,
            IRepository<MenuEntity> menuRepo,
            IBlogAudit audit) : base(logger, settings)
        {
            _menuRepo = menuRepo;
            _audit = audit;
        }

        public async Task<Menu> GetAsync(Guid id)
        {
            var entity = await _menuRepo.GetAsync(id);
            var item = EntityToMenuModel(entity);
            return item;
        }

        public Task<IReadOnlyList<Menu>> GetAllAsync()
        {
            var list = _menuRepo.SelectAsync(p => new Menu
            {
                Id = p.Id,
                DisplayOrder = p.DisplayOrder,
                Icon = p.Icon,
                Title = p.Title,
                Url = p.Url,
                IsOpenInNewTab = p.IsOpenInNewTab
            });

            return list;
        }

        public async Task<Guid> CreateAsync(CreateMenuRequest request)
        {
            var uid = Guid.NewGuid();
            var menu = new MenuEntity
            {
                Id = uid,
                Title = request.Title.Trim(),
                DisplayOrder = request.DisplayOrder,
                Icon = request.Icon,
                Url = request.Url,
                IsOpenInNewTab = request.IsOpenInNewTab
            };

            await _menuRepo.AddAsync(menu);
            await _audit.AddAuditEntry(EventType.Content, AuditEventId.MenuCreated, $"Menu '{menu.Id}' created.");

            return uid;
        }

        public async Task<Guid> UpdateAsync(EditMenuRequest request)
        {
            var menu = await _menuRepo.GetAsync(request.Id);
            if (menu is null)
            {
                throw new InvalidOperationException($"MenuEntity with Id '{request.Id}' not found.");
            }

            var sUrl = SterilizeMenuLink(request.Url.Trim());
            Logger.LogInformation($"Sterilized URL from '{request.Url}' to '{sUrl}'");

            menu.Title = request.Title.Trim();
            menu.Url = sUrl;
            menu.DisplayOrder = request.DisplayOrder;
            menu.Icon = request.Icon;
            menu.IsOpenInNewTab = request.IsOpenInNewTab;

            await _menuRepo.UpdateAsync(menu);
            await _audit.AddAuditEntry(EventType.Content, AuditEventId.MenuUpdated, $"Menu '{request.Id}' updated.");

            return menu.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var menu = await _menuRepo.GetAsync(id);
            if (menu is null)
            {
                throw new InvalidOperationException($"MenuEntity with Id '{id}' not found.");
            }

            _menuRepo.Delete(id);
            await _audit.AddAuditEntry(EventType.Content, AuditEventId.CategoryDeleted, $"Menu '{id}' deleted.");
        }

        private static Menu EntityToMenuModel(MenuEntity entity)
        {
            if (entity is null) return null;

            return new Menu
            {
                Id = entity.Id,
                Title = entity.Title.Trim(),
                DisplayOrder = entity.DisplayOrder,
                Icon = entity.Icon.Trim(),
                Url = entity.Url.Trim(),
                IsOpenInNewTab = entity.IsOpenInNewTab
            };
        }

        public static string SterilizeMenuLink(string rawUrl)
        {
            bool IsUnderLocalSlash()
            {
                // Allows "/" or "/foo" but not "//" or "/\".
                if (rawUrl[0] == '/')
                {
                    // url is exactly "/"
                    if (rawUrl.Length == 1)
                    {
                        return true;
                    }

                    // url doesn't start with "//" or "/\"
                    if (rawUrl[1] != '/' && rawUrl[1] != '\\')
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }

            string invalidReturn = "#";
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                return invalidReturn;
            }

            if (!rawUrl.IsValidUrl())
            {
                return IsUnderLocalSlash() ? rawUrl : invalidReturn;
            }

            var uri = new Uri(rawUrl);
            if (uri.IsLoopback)
            {
                // localhost, 127.0.0.1
                return invalidReturn;
            }

            if (uri.HostNameType == UriHostNameType.IPv4)
            {
                // Disallow LAN IP (e.g. 192.168.0.1, 10.0.0.1)
                if (Utils.IsPrivateIP(uri.Host))
                {
                    return invalidReturn;
                }
            }

            return rawUrl;
        }
    }
}
