using System;
using System.Collections.Generic;
using System.Text;
using ZedGraph;

namespace DSO_Economic
{
    public class CProduction
    {
        public List<CProductionBuilding> buildingGroup;
        public static List<CProductionResource> resourceList;
        public CProduction()
        {
            /*
Wood
Plank
RealWood
RealPlank
ExoticWood
ExoticPlank
Coal
BronzeOre
Bronze
Tool
IronOre
Iron
Steel
GoldOre
Gold
Coin
TitaniumOre
Titanium
Salpeter
Gunpowder
Stone
Marble
Granite
Water
Corn
Fish
Beer
Meat
Sausage
Flour
Bread
Horse
Wheel
Carriage
BronzeSword
IronSword
             */
            this.buildingGroup = new List<CProductionBuilding>();
            resourceList = new List<CProductionResource>();

            resourceList.Add(new CProductionResource("Wood", 0));
            resourceList.Add(new CProductionResource("Plank", 0));
            resourceList.Add(new CProductionResource("RealWood", 0));
            resourceList.Add(new CProductionResource("RealPlank", 0));
            resourceList.Add(new CProductionResource("ExoticWood", 0));
            resourceList.Add(new CProductionResource("ExoticPlank", 0));
            resourceList.Add(new CProductionResource("Coal", 0));
            resourceList.Add(new CProductionResource("BronzeOre", 0));
            resourceList.Add(new CProductionResource("Bronze", 0));
            resourceList.Add(new CProductionResource("Tool", 0));
            resourceList.Add(new CProductionResource("IronOre", 0));
            resourceList.Add(new CProductionResource("Iron", 0));
            resourceList.Add(new CProductionResource("Steel", 0));
            resourceList.Add(new CProductionResource("GoldOre", 0));
            resourceList.Add(new CProductionResource("Gold", 0));
            resourceList.Add(new CProductionResource("Coin", 0));
            resourceList.Add(new CProductionResource("TitaniumOre", 0));
            resourceList.Add(new CProductionResource("Titanium", 0));
            resourceList.Add(new CProductionResource("Salpeter", 0));
            resourceList.Add(new CProductionResource("Gunpowder", 0));
            resourceList.Add(new CProductionResource("Stone", 0));
            resourceList.Add(new CProductionResource("Marble", 0));
            resourceList.Add(new CProductionResource("Granite", 0));
            resourceList.Add(new CProductionResource("Water", 0));
            resourceList.Add(new CProductionResource("Corn", 0));
            resourceList.Add(new CProductionResource("Fish", 0));
            resourceList.Add(new CProductionResource("Beer", 0));
            resourceList.Add(new CProductionResource("Meat", 0));
            resourceList.Add(new CProductionResource("Sausage", 0));
            resourceList.Add(new CProductionResource("Flour", 0));
            resourceList.Add(new CProductionResource("Bread", 0));
            resourceList.Add(new CProductionResource("Horse", 0));
            resourceList.Add(new CProductionResource("Wheel", 0));
            resourceList.Add(new CProductionResource("Carriage", 0));
            resourceList.Add(new CProductionResource("BronzeSword", 0));
            resourceList.Add(new CProductionResource("IronSword", 0));
        }
        public void init()
        {
            foreach (CProductionBuilding pb in this.buildingGroup)
            {
                foreach (CBuildingEntry be in Global.buildingEntries)
                    if (be.Name == pb.name)
                        pb.addBuildingProduce(new CBuildingEntryWrap(be));
                foreach (CProductionResource pr in pb.resourcesNeeded)
                    foreach (CProductionBuilding pb2 in this.findBuildingGroupByRes(pr.name))
                    {
                        pb.addSourceBuilding(pb2);
                        pb2.addTargetBuilding(pb);
                    }
            }
        }
        private List<CProductionBuilding> findBuildingGroupByRes(string resource)
        {
            List<CProductionBuilding> pblist = new List<CProductionBuilding>();
            foreach (CProductionBuilding pb in this.buildingGroup)
                if (pb.resourceProduced.name == resource)
                    pblist.Add(pb);
            return pblist;
        }
        private CProductionResource findResourceByName(string resource)
        {
            foreach (CProductionResource pr in resourceList)
                if (pr.name == resource)
                    return pr;
            return null;
        }
        public PointPairList simulate(string resource)
        {
            DateTime startDate = DateTime.Now;
            for (int i = 0; i < Global.itemEntries.Count; i++)
                resourceList[i].amount = Global.itemEntries[i].amount;
            uint simulationStep;
            PointPairList ppl = new PointPairList();
            List<CProductionBuilding> pblist = findBuildingGroupByRes(resource);
            for (simulationStep = 0; simulationStep < 60 * 60; simulationStep++)
            {
                foreach (CProductionBuilding pb in pblist)
                    if (!pb.simulate(simulationStep))
                        return new PointPairList();

                double diff = new XDate(startDate.AddSeconds(simulationStep));
                ppl.Add(diff, findResourceByName(resource).amount);
            }
            return ppl;
        }

        public class CProductionBuilding
        {
            public string name;
            private List<CBuildingEntryWrap> buildingProduce;
            private List<CProductionBuilding> sourceBuildingGroup;
            private List<CProductionBuilding> targetBuildingGroup;
            public CProductionResource resourceProduced;
            public List<CProductionResource> resourcesNeeded;
            public CProductionBuilding(string name)
            {
                this.name = name;
                this.resourcesNeeded = new List<CProductionResource>();
                this.buildingProduce = new List<CBuildingEntryWrap>();
                this.sourceBuildingGroup = new List<CProductionBuilding>();
                this.targetBuildingGroup = new List<CProductionBuilding>();
            }
            public CProductionBuilding()
            {
                this.buildingProduce = new List<CBuildingEntryWrap>();
                this.sourceBuildingGroup = new List<CProductionBuilding>();
                this.targetBuildingGroup = new List<CProductionBuilding>();
            }
            public void addBuildingProduce(CBuildingEntryWrap bew)
            {
                buildingProduce.Add(bew);
            }
            public void addSourceBuilding(CProductionBuilding pb)
            {
                this.sourceBuildingGroup.Add(pb);
            }
            public void addTargetBuilding(CProductionBuilding pb)
            {
                this.targetBuildingGroup.Add(pb);
            }
            public bool simulate(uint simulationStep)
            {
                return simulate(simulationStep, false, false);
            }
            public bool simulate(uint simulationStep,bool nobackstep,bool noforwardstep)
            {
                if(!nobackstep)
                foreach (CProductionBuilding pb in sourceBuildingGroup)
                    if (!pb.simulate(simulationStep, false, true)) return false;
                foreach (CBuildingEntryWrap be in buildingProduce)
                {
                    if (be.ePTime == -1) return false;
                    if (be.sPTime == -1) return false;
                    if (be.ePTime == 0) return false;
                    if (be.sPTime == 0) return false;
                    double PTime = (be.ePTime - be.sPTime)/1000;
                    if ((simulationStep%PTime==0)||(be.stop))
                    {
                        bool allresourcesfound = true;
                        foreach (CProductionResource pr in resourcesNeeded)
                            foreach (CProductionResource ie in resourceList)
                                if (ie.name == pr.name)
                                    if (be.level * pr.amount > ie.amount)
                                        allresourcesfound=false;
                        if (allresourcesfound)
                        {
                            foreach (CProductionResource pr in resourcesNeeded)
                                foreach (CProductionResource pr2 in resourceList)
                                    if (pr2.name == pr.name)
                                        pr2.amount -= be.level * pr.amount;
                            foreach (CProductionResource pr2 in resourceList)
                                if (pr2.name == resourceProduced.name)
                                    pr2.amount += be.level;
                            be.stop = false;
                        }
                        else
                        {
                            be.stop = true;
                        }
                    }
                }
                if (!noforwardstep)
                    foreach (CProductionBuilding pb in targetBuildingGroup)
                        if (!pb.simulate(simulationStep, true, false)) return false;
                return true;
            }
        }
        public class CBuildingEntryWrap
        {
            private CBuildingEntry buildingEntry;
            public uint level
            {
                get
                {
                    return buildingEntry.level;
                }
            }
            public double sPTime
            {
                get
                {
                    return buildingEntry.sPTime;
                }
            }
            public double ePTime
            {
                get
                {
                    return buildingEntry.ePTime;
                }
            }
            public bool stop;
            public CBuildingEntryWrap(CBuildingEntry be)
            {
                this.buildingEntry = be;
                this.stop = false;
            }
        }
        public class CProductionResource
        {
            public string name;
            public uint amount;
            public CProductionResource()
            {
            }
            public CProductionResource(string name, uint amount)
            {
                this.name = name;
                this.amount = amount;
            }
        }
    }
}
