using System.ComponentModel.DataAnnotations;

        public class User
        {
            [Key]
            public int Id { get; set; }
            public string Name { get; set; }
            public string Ph { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
        }
