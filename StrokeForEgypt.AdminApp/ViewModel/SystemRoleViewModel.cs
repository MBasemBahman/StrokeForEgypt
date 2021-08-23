using StrokeForEgypt.Entity.AuthEntity;
using System.Collections.Generic;
using System.ComponentModel;

namespace StrokeForEgypt.AdminApp.ViewModel
{

    public class SystemRoleViewModel
    {
        public SystemRoleViewModel()
        {
            FullAccessViews = new List<int>();
            ControlAccessViews = new List<int>();
            ViewAccessViews = new List<int>();
            SystemViews = new Dictionary<string, string>();
        }

        public SystemRole SystemRole { get; set; }

        [DisplayName("Full Access Views")]
        public List<int> FullAccessViews { get; set; }

        [DisplayName("Control Access Views")]
        public List<int> ControlAccessViews { get; set; }

        [DisplayName("View Access Views")]
        public List<int> ViewAccessViews { get; set; }

        public Dictionary<string, string> SystemViews { get; set; }

    }
}
