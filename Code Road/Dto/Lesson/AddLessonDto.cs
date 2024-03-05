﻿using System.ComponentModel.DataAnnotations;

namespace Code_Road.Dto.Lesson
{
    public class AddLessonDto
    {

        [Required, MaxLength(50), MinLength(2)]
        public string? Name { get; set; }
        [Required]
        public string? Explanation { get; set; }
        [Required]
        public string? Level { get; set; }
        [Required]
        public string? TopicName { get; set; }
        [Required]
        public FormFileCollection? Images { get; set; }
        public int? Quiz { get; set; }
    }
}
