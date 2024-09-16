using System.Collections.Generic;

namespace DriveAdviser.Core
{
    internal class CriticalSmartAttributes
    {
        public float AttributeWeight { get; set; }
        public int PercentageLimit { get; set; }
        public int StartValue { get; set; }

        public string Name { get; set; }

        //This dictionary holds the values for the SMART attributes that matter.
        //The key is the SMART attribute ID and the values are 
        public static readonly Dictionary<int, CriticalSmartAttributes> CriticalAttributes = new Dictionary<int, CriticalSmartAttributes>
{
        {5,new CriticalSmartAttributes {AttributeWeight=1,PercentageLimit=70, Name = "Reallocated Sector Count"} },
       // {7,new CriticalSmartAttributes {AttributeWeight=.5f,PercentageLimit=20 } },
        {10,new CriticalSmartAttributes {AttributeWeight=3,PercentageLimit=60, Name="Spin Retry Count" } },
        //{177,new CriticalSmartAttributes {AttributeWeight=.1f, PercentageLimit=30,StartValue = 20} },


        { 196,new CriticalSmartAttributes {AttributeWeight=.6f,PercentageLimit=30 } },
        {197, new CriticalSmartAttributes {AttributeWeight=.6f,PercentageLimit=48 } },
        {198,new CriticalSmartAttributes {AttributeWeight=1, PercentageLimit=70 } },
       // {233,new CriticalSmartAttributes {AttributeWeight=.04f, PercentageLimit=50 } }

 };


    }
}
