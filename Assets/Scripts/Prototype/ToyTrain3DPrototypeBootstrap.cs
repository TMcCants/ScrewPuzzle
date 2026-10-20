using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    public sealed class ToyTrain3DPrototypeBootstrap : Radio3DPrototypeBootstrap
    {
        public PlaneScrewLayout Layout { get; private set; }
        protected override bool AllowPitch => true;
        protected override bool StyledTrays => true;
        protected override float ScrewVisualScale => .56f;
        protected override int LayoutSeed => Layout.Seed;
        protected override ScrewColorId[] TraySequence => Layout.Trays;
        protected override string BoardTitle => "THE TOY TRAIN WORKSHOP";
        protected override string BoardId => ThreeDBoardProgress.ToyTrain;
        protected override float ModelVerticalOffset => 190f;
        protected override string WinTitle => "TOY TRAIN CLEARED!";
        protected override void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            Camera.main.gameObject.AddComponent<WorkshopAirportBackdrop>().Configure("Art/Workshop/Station-Background", "Station Background");
            int savedSeed = WorkshopRunSave.Read(BoardId)?.layoutSeed ?? 0;
            Layout = new PlaneScrewLayout(savedSeed > 0 ? savedSeed : Random.Range(1, int.MaxValue));
            radio = new GameObject("Rotating Toy Train").transform;
            var green = Finish(new Color(.12f,.38f,.23f),.12f,.42f);
            var cream = Finish(new Color(.96f,.88f,.69f),.08f,.36f);
            var brass = Finish(new Color(.83f,.56f,.23f),.55f,.5f);
            var dark = Finish(new Color(.07f,.11f,.14f),.05f,.24f);
            var glass = Finish(new Color(.12f,.29f,.37f),.35f,.7f);
            var copper = Finish(new Color(.67f,.30f,.12f),.5f,.42f);
            plates = new Radio3DPlate[PlaneScrewLayout.AssemblyCount];
            string[] names = { "Engine", "Far", "Tender", "Nose", "Boiler", "Drive", "Cab", "Wheels", "Chassis" };
            Vector3[] normals = { Vector3.back,Vector3.forward,Vector3.left,Vector3.right,Vector3.back,Vector3.back,Vector3.up,Vector3.down,Vector3.back };
            for (int f=0; f<plates.Length; f++)
            {
                plates[f] = new GameObject(names[f]+" Assembly").AddComponent<Radio3DPlate>();
                plates[f].transform.SetParent(radio,false);
                if (f >= 6) continue;
                var pos=f==0?new Vector3(.6f,0,-.72f):f==1?new Vector3(.6f,0,.72f):f==2?new Vector3(-2.2f,0,0):f==3?new Vector3(2f,0,0):new Vector3(.6f,0,f==4?-.52f:-.30f);
                RoundedBody(names[f]+" Panel",plates[f].transform,pos,f==2||f==3?new Vector3(.10f,.85f,1.4f):new Vector3(2.6f,.85f,.08f),.035f,f==4?brass:f==5?cream:green);
            }
            var chassis = plates[8].transform;
            RoundedBody("Engine Frame",chassis,new Vector3(.6f,-.45f,0),new Vector3(2.65f,.20f,1.25f),.08f,dark);
            RoundedBody("Tender Frame",chassis,new Vector3(-1.65f,-.45f,0),new Vector3(1.05f,.2f,1.25f),.08f,dark);
            RoundedBody("Coupling",chassis,new Vector3(-.95f,-.43f,0),new Vector3(.5f,.12f,.25f),.05f,brass);
            RoundedBody("Chassis Mount",chassis,new Vector3(.6f,0,.10f),new Vector3(2.6f,.85f,.08f),.035f,green);
            var cab = plates[6].transform;
            RoundedBody("Cab",cab,new Vector3(-.35f,.9f,0),new Vector3(.95f,1.05f,1.3f),.1f,green);
            RoundedBody("Cab Roof",cab,new Vector3(-.35f,1.46f,0),new Vector3(1.2f,.16f,1.5f),.07f,cream);
            var wheels = plates[7].transform;
            RoundedBody("Wheel Carrier",wheels,new Vector3(-.15f,-.60f,0),new Vector3(4.05f,.09f,.38f),.04f,brass);
            foreach(float side in new[]{-1f,1f})
            {
                RoundedBody("Window Frame",cab,new Vector3(-.35f,1.02f,side*.66f),new Vector3(.72f,.65f,.04f),.019f,brass);
                RoundedBody("Cab Window",cab,new Vector3(-.35f,1.02f,side*.69f),new Vector3(.60f,.53f,.025f),.012f,glass);
                RoundedBody("Window Divider",cab,new Vector3(-.35f,1.02f,side*.71f),new Vector3(.035f,.53f,.02f),.009f,cream);
                var shell = plates[side < 0 ? 0 : 1].transform;
                RoundedBody("Boiler Pinstripe",shell,new Vector3(.6f,-.31f,side*.77f),new Vector3(2.35f,.035f,.02f),.009f,brass);
                foreach(float axle in new[]{-1.65f,.2f,1.35f})
                {
                    var wheel=Shape("Train Wheel",PrimitiveType.Cylinder,wheels,new Vector3(axle,-.76f,side*.76f),new Vector3(.65f,.10f,.65f),dark);
                    wheel.localRotation=Quaternion.Euler(90,0,0);
                    var hub=Shape("Brass Hub",PrimitiveType.Cylinder,wheels,new Vector3(axle,-.76f,side*.875f),new Vector3(.36f,.02f,.36f),brass);
                    hub.localRotation=Quaternion.Euler(90,0,0);
                    for(int spoke=0;spoke<4;spoke++)
                    {
                        var bar=RoundedBody("Wheel Spoke",wheels,new Vector3(axle,-.76f,side*.90f),new Vector3(.44f,.035f,.02f),.009f,brass);
                        bar.localRotation=Quaternion.Euler(0,0,spoke*45);
                    }
                }
                RoundedBody("Drive Rod",wheels,new Vector3(.775f,-.86f,side*.94f),new Vector3(1.3f,.065f,.045f),.022f,cream);
            }
            foreach(float axle in new[]{-1.65f,.2f,1.35f})
                RoundedBody("Wheel Axle",wheels,new Vector3(axle,-.76f,0),new Vector3(.10f,.10f,1.6f),.045f,brass);
            RoundedBody("Boiler Roof",plates[0].transform,new Vector3(.9f,.5f,0),new Vector3(2f,.18f,1.45f),.08f,green);
            Shape("Chimney",PrimitiveType.Cylinder,plates[0].transform,new Vector3(1.35f,.84f,0),new Vector3(.35f,.3f,.35f),dark);
            Shape("Chimney Rim",PrimitiveType.Cylinder,plates[0].transform,new Vector3(1.35f,1.14f,0),new Vector3(.5f,.07f,.5f),brass);
            RoundedBody("Tender Body",plates[2].transform,new Vector3(-1.65f,.05f,0),new Vector3(.95f,.85f,1.3f),.1f,green);
            for(int i=0;i<3;i++) RoundedBody("Coal",plates[2].transform,new Vector3(-1.9f+i*.25f,.5f,0),new Vector3(.23f,.25f,.75f),.1f,dark);
            RoundedBody("Boiler Pipe",plates[4].transform,new Vector3(.6f,-.23f,-.59f),new Vector3(1.9f,.12f,.08f),.035f,dark);
            var motor=Shape("Copper Motor",PrimitiveType.Cylinder,plates[4].transform,new Vector3(.6f,-.13f,-.61f),new Vector3(.23f,.32f,.23f),copper);
            motor.localRotation=Quaternion.Euler(0,0,90);
            for(int i=0;i<3;i++)
            {
                var center=new Vector3(.1f+i*.5f,-.23f,-.37f);
                var gear=Shape("Drive Gear",PrimitiveType.Cylinder,plates[5].transform,center,new Vector3(.28f,.025f,.28f),brass);
                gear.localRotation=Quaternion.Euler(90,0,0);
                for(int t=0;t<10;t++)
                {
                    float a=t*Mathf.PI/5;
                    var tooth=RoundedBody("Gear Tooth",plates[5].transform,center+new Vector3(Mathf.Cos(a),Mathf.Sin(a),0)*.14f,new Vector3(.065f,.05f,.045f),.02f,brass);
                    tooth.localRotation=Quaternion.Euler(0,0,t*36);
                }
            }
            var colors=new[]{Finish(new Color(.9f,.12f,.08f),.2f,.45f),Finish(new Color(.05f,.4f,.95f),.2f,.45f),Finish(new Color(1f,.7f,.04f),.2f,.45f)};
            Vector3[] roofSockets={new Vector3(-.65f,1.59f,-.45f),new Vector3(-.05f,1.59f,-.45f),new Vector3(-.35f,1.59f,.45f)};
            Vector3[] underSockets={new Vector3(-1.95f,-.69f,0),new Vector3(-.55f,-.69f,0),new Vector3(.75f,-.69f,0)};
            screws=new List<Radio3DScrew>();
            for(int f=0;f<plates.Length;f++)
            {
                var fasteners=new Radio3DScrew[3];
                for(int i=0;i<3;i++)
                {
                    int index=f*3+i, socket=Layout.Sockets[index];
                    Vector3 pos=f==6?roofSockets[socket]:f==7?underSockets[socket]:
                        f==2||f==3?new Vector3(f==2?-2.30f:2.10f,.14f,(socket-1)*.48f):
                        new Vector3(.6f+(socket-1)*.86f,.16f,f==0?-.83f:f==1?.83f:f==4?-.63f:f==5?-.41f:-.01f);
                    var color=Layout.Colors[index];
                    fasteners[i]=Screw(names[f]+" "+i,radio,pos,normals[f],colors[(int)color],dark);
                    int cover=PlaneScrewLayout.Covers[f];
                    fasteners[i].Initialize(color,cover<0?null:plates[cover]);
                    screws.Add(fasteners[i]);
                }
                plates[f].Configure(fasteners,normals[f]);
            }
            radio.localRotation=Quaternion.Euler(0,20,0);
        }
        protected override void DecorateBoardUi(RectTransform parent)
        {
            foreach(var region in new[]{new Vector2(740,440)})
            {
                var panel=new GameObject("Station UI Backing",typeof(RectTransform),typeof(UnityEngine.UI.Image));
                panel.transform.SetParent(parent,false);
                var rect=panel.GetComponent<RectTransform>(); rect.sizeDelta=new Vector2(1080,region.y); rect.anchoredPosition=new Vector2(0,region.x);
                var image=panel.GetComponent<UnityEngine.UI.Image>(); image.color=new Color(.08f,.12f,.15f,.84f); image.raycastTarget=false;
            }
        }
        private Material Finish(Color color,float metallic,float smoothness)
        {
            var material=Material(color); material.SetFloat("_Metallic",metallic);
            if(material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness",smoothness);
            if(material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness",smoothness);
            return material;
        }
    }
}
