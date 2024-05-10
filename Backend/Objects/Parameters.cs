using ItemFinder_WPF.Backend.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend.Objects
{
    public class Parameters
    {
        public string Query { get; set; }
        public double MeanLowerTolerance { get; set; }
        public double MeanUpperTolerance { get; set; }
        public bool SellerGradeFiltering { get; set; }
        public double MinimumSellerGrade { get; set; }
        public bool DeliveryAvailable { get; set; }
        public bool ExcludeOngoingPurchase { get; set; }
        public List<State> ProductCondition { get; set; }
        public FilterBy AdSorting { get; set; }
        public bool LocationEnabled { get; set; }
        public uint ZipCode { get; set; }
        public MeanCalculation MeanCalculation { get; set; }
        public int PersonalizedMean { get; set; }
        public bool SaveSearch { get; set; }
        public bool CreateAlert { get; set; }
        public bool BlacklistEnabled { get; set; }
        public bool MeanToleranceEnabled { get; set; } 
        public string UUID { get; set; }
        public List<string> Blacklist { get; set; }
    }
}
