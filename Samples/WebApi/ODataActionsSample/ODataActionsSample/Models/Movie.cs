using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ODataActionsSample.Models
{
    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public DateTime? DueDate { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }   

        public bool IsCheckedOut 
        {
            get { return DueDate.HasValue; }
        }
    }
}