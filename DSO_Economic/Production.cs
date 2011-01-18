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
        private List<CProductionStep> productionSteps;
        private uint max;
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
SteelSword
TitaniumSword
Bow
Longbow
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
            resourceList.Add(new CProductionResource("SteelSword", 0));
            resourceList.Add(new CProductionResource("TitaniumSword", 0));
            resourceList.Add(new CProductionResource("Bow", 0));
            resourceList.Add(new CProductionResource("Longbow", 0));
        }
        public void init()
        {
            foreach (CProductionBuilding pb in this.buildingGroup)
            {
                foreach (CBuildingEntry be in Global.buildingEntries)
                    if (be.Name == pb.name)
                        pb.addBuildingProduce(new CBuildingEntryWrap(be));
                foreach (CProductionResource pr in pb.resourcesNeeded)
                    foreach (CProductionBuilding pb2 in this.findBuildingGroupByResProduced(pr.name))
                    {
                        pb.addSourceBuilding(pb2);
                        pb2.addTargetBuilding(pb);
                    }
            }
        }
        private List<CProductionBuilding> getBuildingChain(string resource)
        {
            List<CProductionBuilding> pblist = new List<CProductionBuilding>();
            foreach (CProductionBuilding pb in findBuildingGroupByResProduced(resource))
                pblist.Add(pb);
            if (pblist.Count == 0)
                return new List<CProductionBuilding>();
            int oldcount = pblist.Count;
            do
            {
                oldcount = pblist.Count;
                for (int i = 0; i < pblist.Count; i++)
                {
                    CProductionBuilding pb = pblist[i];
                    foreach (CProductionBuilding pb2 in pb.sourceBuildingGroup)
                        if (!pblist.Contains(pb2))
                            pblist.Add(pb2);
                    foreach (CProductionBuilding pb2 in pb.targetBuildingGroup)
                        if (!pblist.Contains(pb2))
                            pblist.Add(pb2);
                }
            } while (oldcount != pblist.Count);
            return pblist;
        }
        private List<CProductionBuilding> findBuildingGroupByResProduced(string resource)
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
        public bool createProductionSteps(string resource)
        {
            return createProductionSteps(resource, 60 * 60);
        }
        public bool createProductionSteps(string resource,uint timelimit)
        {
            productionSteps = new List<CProductionStep>();
            for (int i = 0; i < Global.itemEntries.Count; i++)
                resourceList[i].amount = Global.itemEntries[i].amount;
            max = Global.itemEntries[0].max;
            foreach (CProductionBuilding pb in getBuildingChain(resource))
                foreach (CBuildingEntryWrap bew in pb.buildingProduce)
                {
                    if ((bew.ePTime == -1) || (bew.sPTime == -1))
                        return false;
                    if ((bew.ePTime == 0) || (bew.sPTime == 0))
                        return false;
                    double PTime = bew.ePTime - bew.sPTime;
                    uint ss = (uint)(PTime / 1000);
                    for (uint i = 0; i < timelimit; i += ss)
                        productionSteps.Add(new CProductionStep(i, bew));

                }
            productionSteps.Sort(SimulationStepSort.Comparison);
            if (productionSteps.Count == 0) return false;
            return true;
        }

        public double findLimitEmpty(string resource)
        {
            return findLimit(resource, false);
        }
        public double findLimitFull(string resource)
        {
            return findLimit(resource, true);
        }
        public double findLimit(string resource,bool full)
        {
            uint lastSimulationStep = 0;
            uint timelimit=8 * 60 * 60;
            if (!createProductionSteps(resource, timelimit)) return -1;

            for (int i = 0; i < productionSteps.Count; i++)
            {
                CProductionStep ps = productionSteps[i];
                bool allresourcesfound = true;
                foreach (CProductionResource pr in ps.productionBuilding.resourcesNeeded)
                    foreach (CProductionResource ie in resourceList)
                        if (ie.name == pr.name)
                            if (ps.productionBuilding.level * pr.amount > ie.amount)
                                allresourcesfound = false;
                if (!allresourcesfound)
                {
                    if (ps.simulationStep++ > timelimit)
                        continue;
                    for (int j = 0; j < productionSteps.Count - 1; j++)
                    {
                        if (productionSteps[j].simulationStep >= ps.simulationStep)
                        {
                            productionSteps[j] = ps;
                            break;
                        }
                        productionSteps[j] = productionSteps[j + 1];
                    }
                    i--;
                    continue;
                }
                else
                {
                    foreach (CProductionResource pr2 in ps.productionBuilding.resourcesNeeded)
                        foreach (CProductionResource pr3 in resourceList)
                            if (pr2.name == pr3.name)
                                pr3.amount -= ps.productionBuilding.level * pr2.amount;
                    foreach (CProductionResource pr2 in resourceList)
                        if (pr2.name == ps.productionBuilding.resourceProduced.name)
                            pr2.amount += ps.productionBuilding.level;
                }
                if (productionSteps[i].simulationStep > lastSimulationStep)
                {
                    CProductionResource res = findResourceByName(resource);
                    if (!full)
                        if (res.amount == 0)
                            return productionSteps[i].simulationStep;
                        else
                            if (res.amount >= max)
                                return -1;
                    if (full)
                        if (res.amount >= max)
                            return productionSteps[i].simulationStep;
                        else
                            if (res.amount == 0)
                                return -1;
                    lastSimulationStep = productionSteps[i].simulationStep;
                }
            }
            return -1;
        }
        public PointPairList simulate(string resource)
        {
            DateTime startDate = DateTime.Now;
            PointPairList ppl = new PointPairList();
            uint lastSimulationStep = 0;

            if (!createProductionSteps(resource)) return new PointPairList();

            for (int i = 0; i < productionSteps.Count; i++)
            {
                CProductionStep ps = productionSteps[i];
                bool allresourcesfound = true;
                foreach (CProductionResource pr in ps.productionBuilding.resourcesNeeded)
                    foreach (CProductionResource ie in resourceList)
                        if (ie.name == pr.name)
                            if (ps.productionBuilding.level * pr.amount > ie.amount)
                                allresourcesfound = false;
                if (!allresourcesfound)
                {
                    if(ps.simulationStep++>60 * 60)
                        continue;
                    for (int j = 0; j < productionSteps.Count - 1; j++)
                    {
                        if (productionSteps[j].simulationStep >= ps.simulationStep)
                        {
                            productionSteps[j] = ps;
                            break;
                        }
                        productionSteps[j] = productionSteps[j + 1];
                    }
                    i--;
                    continue;
                }
                else
                {
                    foreach (CProductionResource pr2 in ps.productionBuilding.resourcesNeeded)
                        foreach (CProductionResource pr3 in resourceList)
                            if (pr2.name == pr3.name)
                                pr3.amount -= ps.productionBuilding.level * pr2.amount;
                    foreach (CProductionResource pr2 in resourceList)
                        if (pr2.name == ps.productionBuilding.resourceProduced.name)
                            pr2.amount += ps.productionBuilding.level;
                }
                if (productionSteps[i].simulationStep > lastSimulationStep)
                {
                    double diff = new XDate(startDate.AddSeconds(productionSteps[i].simulationStep));
                    ppl.Add(diff, findResourceByName(resource).amount);
                    lastSimulationStep = productionSteps[i].simulationStep;
                }
            }
            
            return ppl;
        }

        public class CProductionBuilding : IEquatable<CProductionBuilding>
        {
            public string name;
            public List<CBuildingEntryWrap> buildingProduce;
            public List<CProductionBuilding> sourceBuildingGroup;
            public List<CProductionBuilding> targetBuildingGroup;
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
            public bool Equals(CProductionBuilding obj)
            {
                if (obj.name == this.name)
                    return true;
                else
                    return false;
            }
            public CProductionBuilding()
            {
                this.buildingProduce = new List<CBuildingEntryWrap>();
                this.sourceBuildingGroup = new List<CProductionBuilding>();
                this.targetBuildingGroup = new List<CProductionBuilding>();
            }

            public void addBuildingProduce(CBuildingEntryWrap bew)
            {
                bew.resourcesNeeded = this.resourcesNeeded;
                bew.resourceProduced = this.resourceProduced;
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
        }
        public class CProductionStep
        {
            public uint simulationStep;
            public CBuildingEntryWrap productionBuilding;
            public CProductionStep(uint simulationStep, CBuildingEntryWrap productionBuilding)
            {
                this.simulationStep = simulationStep;
                this.productionBuilding = productionBuilding;
            }
        }
        public class CBuildingEntryWrap
        {
            public List<CProductionResource> resourcesNeeded;
            public CProductionResource resourceProduced;
            private CBuildingEntry buildingEntry;
            private uint _level=0;
            private double _sPTime = -1;
            private double _ePTime = -1;
            public uint level
            {
                get
                {
                    if (_level == 0)
                        _level=buildingEntry.level;
                    return _level;
                }
            }
            public double sPTime
            {
                get
                {
                    if (this._sPTime == -1)
                        this._sPTime = buildingEntry.sPTime;
                    return this._sPTime;
                }
            }
            public double ePTime
            {
                get
                {
                    if (this._ePTime == -1)
                        this._ePTime = buildingEntry.ePTime;
                    return this._ePTime;
                }
            }
            public bool stop;
            public CBuildingEntryWrap()
            {
            }
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
        public class SimulationStepSort
        {
            public static int Comparison(CProductionStep x, CProductionStep y)
            {
                return x.simulationStep.CompareTo(y.simulationStep);
            }
        }
    }
}
