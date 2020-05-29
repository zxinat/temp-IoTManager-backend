using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.Serializers
{
    public class PersonSerializers
    {
        public string staffId { get; set; }
        public string name { get; set; }
        public string gengder { get; set; }
        public string type { get; set; }
        public PersonSerializers()
        {

        }
        public PersonSerializers(StaffModel staff)
        {
            staffId = staff.staffId;
            name = staff.staffName;
            gengder = staff.gender;
            type = "员工";
        }
        public PersonSerializers(CustomerModel customer)
        {
            staffId = customer.staffId;
            name = customer.name;
            gengder = customer.gender;
            type = "访客";
        }
    }
}
