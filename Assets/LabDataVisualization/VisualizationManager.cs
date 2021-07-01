using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LabData;
using UnityEngine;

namespace LabVisualization
{
    public class VisualizationManager : MonoSingleton<VisualizationManager>
    {
        private List<VisualControllerBase> _visualControllerList;

        private List<IEquipment> Equipments;

        public void StartDataVisualization()
        {
            var temp = FindObjectsOfType<VisualControllerBase>().ToList();
            _visualControllerList = temp.Count > 0 ? temp : null;
            _visualControllerList?.ForEach(p =>
            {
                p.VisualInit();
            });
            Equipments?.ForEach(p =>
            {
                p.EquipmentStart();
            });
            _visualControllerList?.ForEach(p => p.VisualShow());
        }

        public void VisulizationInit()
        {
            Equipments = FindObjectsOfType<MonoBehaviour>().OfType<IEquipment>().ToList();
            Equipments?.ForEach(p =>
            {
                p.EquipmentInit();
            });
        }




       

        public void DisposeDataVisualization()
        {
            _visualControllerList?.ForEach(p => p.VisualDispose());
            Equipments?.ForEach(p =>
            {
                p.EquipmentStop();
            });
        }
    }

}

