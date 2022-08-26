using System.Collections.Generic;

namespace TasksApp.Models
{
    public class BobCatVM
    {
        public IEnumerable<BobCat> BobCatList { get; set; }
        public BobCat BobCatModel {get;set;}
    }
}
