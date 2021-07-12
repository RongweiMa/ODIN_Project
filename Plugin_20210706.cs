using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using Caliburn.Micro;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;

using VisualComponents.eCatalogue.DataModel.Data;
using VisualComponents.eCatalogue.DataModel.Data.Entities;

namespace Tutorial1
{
    [Export(typeof(IPlugin))]
    public class Class1 : IPlugin
    {
        [Import]
        private IApplication app = null;

        [ImportingConstructor]
        public Class1([Import(typeof(IApplication))] IApplication app)
        {
            this.app = app;
        }

        void IPlugin.Exit()
        {
        }

        /// start from IPlugin.Initialize
        void IPlugin.Initialize()
        {
            IMessageService ms = IoC.Get<IMessageService>();
            ms.AppendMessage("Print two components: one robot and one gripper", MessageLevel.Warning);
            /// var loadedComp = AddComponent(IoC.Get<IApplication>(), "59753353-f21b-4f9c-8e0a-87b9802cf70b");
            var loadedComp = AddComponent(IoC.Get<IApplication>(), "3cf363e8-c1a3-4cc1-a641-76a4518e852a");

            /// var loadedComp1 = AddComponent1(IoC.Get<IApplication>(), "c814b0ef-7445-4c4c-a76e-440d414be392");
            ISimComponent comp = app.World.Components.First();
            Matrix currentPosition = comp.TransformationInWorld;
            currentPosition.SetP(new Vector3(1030, 0, 1260));
            currentPosition.SetWPR(new Vector3(0, 90, 0));
            comp.TransformationInWorld = currentPosition;

            ///ISimWorld simWorld = app.World;
            ///ISimComponent comp = simWorld.CreateComponent("Example2");
            ///
            var loadedComp1 = AddComponent(IoC.Get<IApplication>(), "771eed49-9d89-44cd-a02c-8c930c84afe2");
        }

        private List<Item> eCatComponentCache;

        public ISimComponent AddComponent(IApplication app, string componentVCID)
        {


            /*
            ISimWorld simWorld = app.World;
            ISimComponent comp = simWorld.CreateComponent(componentVCID);
            Matrix currentPosition = comp.TransformationInWorld;
            currentPosition.SetP(new Vector3(400, 0, 0));
            comp.TransformationInWorld = currentPosition;
            */

            if (eCatComponentCache == null)
            {
                eCatComponentCache = EnumerateECatComponents();
            }

            var eCatItem = eCatComponentCache.FirstOrDefault(i => i.VCID == componentVCID && i.IsLocalItem);
            if (eCatItem == null) throw new ArgumentException("Unknown VCID");

            ISimComponent[] loadedComponents = app.LoadLayout(new Uri(eCatItem.FileUri));
            return loadedComponents.FirstOrDefault();
        }


        /// <summary>
        /// Queries the local eCatalog SQL database for all components.
        /// </summary>
        /// <returns></returns>
        private List<Item> EnumerateECatComponents()
        {
            var eCatContext = IoC.Get<IECatDataContext>();
            var eCatItems = eCatContext.EnabledItems;

            // Need to get all to a list first so Entity Framework doesn't try to convert the Where clause to SQL.
            return eCatItems.ToList()
                            .Where(i => !i.IsDeprecated && i.ModelType == "Component")
                            .ToList();
        }

    }


}
