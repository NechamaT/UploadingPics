using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileUploading.Data;

namespace FileUploading.Models
{
    public class ViewImageViewModel
    {
        public  Image Image { get; set; }
        public  bool ShowImage { get; set; }
        public bool ShowMessage { get; set; }
    }
}
