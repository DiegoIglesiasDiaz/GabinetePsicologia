using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class InformeTrastorno
    {
        public InformeTrastorno()
        {

        }

        public Guid Id {  get; set; }
        [ForeignKey("Trastornos")]
        public Guid TrastornoId { get; set; }
        [ForeignKey("Informes")]
        public Guid InformeId {  get; set; }
        public int Severidad { get; set; }
        public string TrastornoName { get; set; }
        public string TrastornoTipo { get; set; }
    }
}
