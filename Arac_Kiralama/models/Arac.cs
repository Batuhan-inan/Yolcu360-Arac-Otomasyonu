using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arac_Kiralama.models
{
    public class Arac
    {
        public int Id { get; set; }
        public int KoleksiyonId { get; set; }
        public string AracModeli { get; set; }
        public string KiralamaSirketi { get; set; }
        public string VitesTipi { get; set; }
        public string YakitTipi { get; set; }
        public string Fiyat { get; set; }
    }
}
