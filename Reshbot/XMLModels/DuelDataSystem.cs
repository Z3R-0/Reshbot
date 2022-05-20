using ReshUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reshbot.XMLModels {

    public class Duel : IModel {
        public int Id;
        public string Challenger;
        public string Challenged;

        public Duel(int id) {
            Id = id;
        }
    }

    public class DuelDataSystem : IXMLDataSystem {
        public DuelDataSystem(Type dataModel) : base(dataModel) { }

        private static DuelDataSystem _instance;

        public static DuelDataSystem instance {
            get {
                if (_instance != null)
                    _instance = new DuelDataSystem(typeof(Duel));

                return _instance;
            }

            private set { _instance = value; }
        }
    }
}
