﻿using System.ComponentModel.DataAnnotations;

namespace Moonglade.Core.CategoryFeature
{
    public class EditCategoryRequest
    {
        [Required]
        [Display(Name = "Display Name")]
        [MaxLength(64)]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Route Name")]
        [RegularExpression("(?!-)([a-z0-9-]+)", ErrorMessage = "Route Name can only accept lower case English letters (a-z) and numbers (0-9) with/out hyphen (-) in middle.")]
        [MaxLength(64)]
        public string RouteName { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(128)]
        public string Note { get; set; }
    }
}
