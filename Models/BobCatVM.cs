using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApp.Models
{
    public class BobCatVM
    {
        public IEnumerable<BobCat> BobCatList { get; set; }

        public BobCat BobCatModel { get; set; }
    }
}
