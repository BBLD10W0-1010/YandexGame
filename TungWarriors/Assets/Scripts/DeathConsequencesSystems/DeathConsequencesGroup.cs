using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.DeathConsequencesSystems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProcessDamageThisFrameSystem))]
    public partial class DeathConsequencesGroup : ComponentSystemGroup { }
}
