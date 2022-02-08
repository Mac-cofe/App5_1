using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App5_1
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public DelegateCommand SelectCommand1 { get; }
        public DelegateCommand SelectCommand2 { get; }
        public DelegateCommand SelectCommand3 { get; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SelectCommand1 = new DelegateCommand(OnSelectCommand1);
            SelectCommand2 = new DelegateCommand(OnSelectCommand2);
            SelectCommand3 = new DelegateCommand(OnSelectCommand3);
        }
        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }
        
        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }
        private void OnSelectCommand1()
        {
            RaiseHideRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();
            TaskDialog.Show("Количество труб", $"Количество труб: {pipes.Count.ToString()}");
            RaiseShowRequest();
        }
        private void OnSelectCommand2()
        {
            RaiseHideRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            var walls = new FilteredElementCollector(doc)
               .OfClass(typeof(Wall))
               .Cast<Wall>()
               .ToList();
            double Sum = 0;
            foreach (var wall in walls)
            {
                Parameter volumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (volumeParameter.StorageType == StorageType.Double)
                {
                    double volumeValue = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                    Sum += volumeValue;
                }
            }
            TaskDialog.Show("Объем стен", $"Объем стен: {Sum} м³");
            RaiseShowRequest();
        }
        private void OnSelectCommand3()
        {
            RaiseHideRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<FamilyInstance> familyInstances = new FilteredElementCollector(doc)
               .OfCategory(BuiltInCategory.OST_Doors)
               .WhereElementIsNotElementType()
               .Cast<FamilyInstance>()
               .ToList();
            TaskDialog.Show("Количество дверей", $"Количество дверей: {familyInstances.Count.ToString()}");
            RaiseShowRequest();
        }
    }
}
