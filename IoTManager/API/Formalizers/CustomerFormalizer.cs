using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTManager.API.Formalizers
{
    public class CustomerFormalizer
    {
        public CustomerFormalizer()
        {
            gender = null;
            remark = null;
            affiliation = null;
        }
        public string staffId { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string phoneNumber { get; set; }
        public string affiliation { get; set; }
        public string cause { get; set; }
        public string remark { get; set; }
    }
}
