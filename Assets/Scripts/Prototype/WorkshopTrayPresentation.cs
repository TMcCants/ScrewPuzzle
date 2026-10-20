using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>Shared sorting boxes and visible lock feedback. No ad or payment integration.</summary>
    public sealed class WorkshopTrayPresentation : MonoBehaviour
    {
        private Radio3DPuzzle puzzle;
        private Radio3DInteraction input;
        private RectTransform layout;
        private Canvas canvas;
        private Sprite rounded, circle, padlock;
        private readonly List<Texture2D> textures = new List<Texture2D>();
        private readonly Image[] rims = new Image[4];
        private readonly Text[] labels = new Text[4];
        private readonly Image[,] holes = new Image[4,3];
        private readonly GameObject[,] pluses = new GameObject[4,3];
        private readonly RectTransform[] lids = new RectTransform[4];
        private readonly CanvasGroup[] lidGroups = new CanvasGroup[4];
        private readonly Button[] lidButtons = new Button[4];
        private readonly bool[] wasOpen = new bool[4];
        private readonly float[] lift = new float[4];
        private GameObject dialog;
        private int pendingTray;
        private bool initialized;
        private readonly List<ScrewView> screwViews = new List<ScrewView>();
        private MaterialPropertyBlock block;
        private static readonly Color Ink = new Color(.25f,.20f,.14f);
        private static readonly Color Cream = new Color(.98f,.94f,.83f);
        private sealed class ScrewView
        {
            public Radio3DScrew Screw;
            public Renderer Head;
            public Color Color;
            public Image Marker;
        }

        public void Configure(RectTransform parent, Radio3DPuzzle board, Radio3DInteraction interaction)
        {
            block=new MaterialPropertyBlock();
            layout=parent; canvas=parent.GetComponentInParent<Canvas>(); puzzle=board; input=interaction;
            input.ExplainLockedScrews=true;
            rounded=MakeSprite(0); circle=MakeSprite(1); padlock=MakeSprite(2);
            var progressBacking=Box("Progress Backing",layout,new Vector2(0,-749),new Vector2(1000,64),new Color(.08f,.12f,.15f,.72f));
            progressBacking.transform.SetAsFirstSibling();
            for(int i=0;i<4;i++) BuildTray(i);
            Box("Legend Backing",layout,new Vector2(0,-219),new Vector2(1000,42),new Color(.08f,.12f,.15f,.82f));
            TextAt("Screw State Legend",layout,"BRIGHT: AVAILABLE   •   LOCK: REMOVE A PART FIRST",new Vector2(0,-219),new Vector2(1000,40),23,new Color(.98f,.94f,.83f));
            BuildDialog();
            foreach(var screw in input.Radio.GetComponentsInChildren<Radio3DScrew>())
            {
                var head=screw.transform.Find("Head Visual").GetComponent<Renderer>();
                var marker=Box("Lock Marker " + screw.name,layout,Vector2.zero,new Vector2(52,52),new Color(.08f,.12f,.13f,.94f),circle);
                Box("Padlock",marker.transform,Vector2.zero,new Vector2(30,38),new Color(1,.88f,.57f),padlock);
                marker.raycastTarget=false;
                screwViews.Add(new ScrewView { Screw=screw,Head=head,Color=head.sharedMaterial.color,Marker=marker });
            }
            // Keep the modal above screw markers and tray content.
            dialog.transform.SetAsLastSibling();
            puzzle.HoleScreenPosition=(tray,hole)=>RectTransformUtility.WorldToScreenPoint(UiCamera,holes[tray,hole].transform.position);
            puzzle.Restarted+=OnRestart;
        }
        private Camera UiCamera => canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
        private void BuildTray(int i)
        {
            Vector2 position=new Vector2(i%2==0?-248:248,i<2?-365:-605);
            Box("Tray Shadow " + i,layout,position+new Vector2(0,-9),new Vector2(464,216),new Color(.05f,.08f,.08f,.35f));
            var rim=Box("Tray " + (i+1),layout,position,new Vector2(460,214),Cream); rims[i]=rim;
            rim.raycastTarget=true;
            var card=Box("Tray Interior",rim.transform,new Vector2(0,3),new Vector2(442,194),Cream);
            labels[i]=TextAt("Tray Label",card.transform,"",new Vector2(0,63),new Vector2(400,45),28,Ink);
            for(int h=0;h<3;h++)
            {
                Vector2 pos=new Vector2((h-1)*127,-21);
                Box("Socket Lip",card.transform,pos,new Vector2(103,103),new Color(.82f,.75f,.60f),circle);
                Box("Socket Shadow",card.transform,pos+new Vector2(0,3),new Vector2(88,88),new Color(.30f,.28f,.23f),circle);
                holes[i,h]=Box("Socket " + i + " " + h,card.transform,pos+new Vector2(0,-1),new Vector2(77,77),new Color(.49f,.45f,.36f),circle);
                var plus=Box("Collected Plus",holes[i,h].transform,Vector2.zero,new Vector2(35,8),Cream);
                Box("Cross",plus.transform,Vector2.zero,new Vector2(8,35),Cream);
                pluses[i,h]=plus.gameObject;
            }
            if(i<2)return;
            var lid=Box("Tray Lid " + i,rim.transform,Vector2.zero,new Vector2(460,214),new Color(.91f,.85f,.71f));
            lids[i]=lid.rectTransform; lidGroups[i]=lid.gameObject.AddComponent<CanvasGroup>();
            Box("Lid Face",lid.transform,new Vector2(0,4),new Vector2(442,194),Cream);
            Box("Lid Lock",lid.transform,new Vector2(0,30),new Vector2(44,52),new Color(.52f,.39f,.23f),padlock);
            TextAt("Lid Caption",lid.transform,"BONUS TRAY",new Vector2(0,-22),new Vector2(400,36),24,Ink);
            var button=Box("Unlock Tray " + i,lid.transform,new Vector2(0,-69),new Vector2(236,48),new Color(.21f,.39f,.32f));
            button.raycastTarget=true;
            int index=i; lidButtons[i]=button.gameObject.AddComponent<Button>();
            lidButtons[i].onClick.AddListener(()=>ShowUnlock(index));
            TextAt("Unlock Label",button.transform,"UNLOCK",Vector2.zero,new Vector2(225,43),24,Cream);
        }
        private void BuildDialog()
        {
            var shade=Box("Bonus Tray Preview",layout,Vector2.zero,new Vector2(12000,12000),new Color(.04f,.07f,.08f,.8f),null);
            shade.raycastTarget=true; dialog=shade.gameObject;
            var card=Box("Unlock Card",shade.transform,Vector2.zero,new Vector2(880,500),Cream);
            TextAt("Title",card.transform,"BONUS TRAY",new Vector2(0,170),new Vector2(800,65),42,Ink);
            TextAt("Explanation",card.transform,"Ad and credit unlocks aren't connected yet.\nPreview opening this tray for free.",new Vector2(0,55),new Vector2(780,130),30,Ink);
            AddButton("Preview Open",card.transform,"PREVIEW OPEN",new Vector2(0,-75),()=>
            {
                if(puzzle.CanInteract && puzzle.UnlockTestTray(pendingTray)) dialog.SetActive(false);
            });
            AddButton("Cancel Unlock",card.transform,"NOT NOW",new Vector2(0,-170),()=>dialog.SetActive(false));
            dialog.SetActive(false);
        }
        private void AddButton(string name,Transform parent,string caption,Vector2 position,UnityEngine.Events.UnityAction action)
        {
            var image=Box(name,parent,position,new Vector2(630,72),new Color(.21f,.39f,.32f)); image.raycastTarget=true;
            image.gameObject.AddComponent<Button>().onClick.AddListener(action);
            TextAt(name+" Label",image.transform,caption,Vector2.zero,new Vector2(600,65),30,Cream);
        }
        private void ShowUnlock(int index)
        {
            if(!puzzle.CanInteract || puzzle.Trays[index].IsOpen || !puzzle.HasQueuedTrays)return;
            input.CancelPointer(); GetComponent<WorkshopHint>()?.Dismiss();
            pendingTray=index; dialog.SetActive(true); dialog.transform.SetAsLastSibling();
        }
        private void OnRestart()
        {
            initialized=false; dialog.SetActive(false);
        }
        private void LateUpdate()
        {
            if(puzzle==null)return;
            for(int i=0;i<4;i++)
            {
                var tray=puzzle.Trays[i];
                Color color=tray.HasColor?TrayColor(tray.Color):new Color(.60f,.65f,.54f);
                rims[i].color=color;
                labels[i].text=tray.HasColor?tray.Color.ToString().ToUpperInvariant()+"   "+tray.Count+"/3":"COMPLETE";
                for(int h=0;h<3;h++)
                {
                    holes[i,h].color=h<tray.Count?color:new Color(.49f,.45f,.36f);
                    pluses[i,h].SetActive(h<tray.Count);
                }
                if(i<2)continue;
                if(!initialized)lift[i]=tray.IsOpen?1:0;
                else if(tray.IsOpen && !wasOpen[i])lift[i]=0;
                if(tray.IsOpen)lift[i]=Mathf.Min(1,lift[i]+Time.unscaledDeltaTime/.45f); else lift[i]=0;
                float t=Mathf.SmoothStep(0,1,lift[i]);
                lids[i].gameObject.SetActive(t<1);
                lids[i].anchoredPosition=new Vector2(0,95*t);
                lids[i].localScale=new Vector3(1,1-.7f*t,1);
                lidGroups[i].alpha=1-t;
                lidButtons[i].interactable=puzzle.CanInteract && !tray.IsOpen && puzzle.HasQueuedTrays;
                lidButtons[i].GetComponentInChildren<Text>().text=puzzle.HasQueuedTrays?"UNLOCK":"NOT NEEDED";
                wasOpen[i]=tray.IsOpen;
            }
            initialized=true;
            Physics.SyncTransforms();
            foreach(var item in screwViews)
            {
                bool locked=!item.Screw.IsAccessible;
                block.Clear();
                Color color=locked?Color.Lerp(item.Color,new Color(.38f,.37f,.33f),.6f)*.8f:item.Color;
                color.a=1;
                block.SetColor("_Color",color); block.SetColor("_BaseColor",color);
                block.SetFloat("_Glossiness",locked?.08f:.45f); block.SetFloat("_Smoothness",locked?.08f:.45f);
                item.Head.SetPropertyBlock(block);
                Vector3 point=input.View.WorldToScreenPoint(item.Screw.transform.position);
                bool visible=locked && !item.Screw.IsRemoved && point.z>0 && !dialog.activeSelf &&
                    Vector3.Dot(item.Screw.transform.up,(input.View.transform.position-item.Screw.transform.position).normalized)>.35f &&
                    input.PickVisibleScrew(point)==item.Screw;
                item.Marker.gameObject.SetActive(visible);
                if(visible)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(layout,point,UiCamera,out Vector2 local);
                    item.Marker.rectTransform.anchoredPosition=local+new Vector2(30,38);
                }
            }
        }
        private static Color TrayColor(ScrewColorId color) => color==ScrewColorId.Red?new Color(.83f,.27f,.20f):color==ScrewColorId.Blue?new Color(.20f,.48f,.78f):new Color(.93f,.68f,.16f);
        private Image Box(string name,Transform parent,Vector2 position,Vector2 size,Color color,Sprite sprite=null)
        {
            var obj=new GameObject(name,typeof(RectTransform),typeof(Image)); obj.transform.SetParent(parent,false);
            var image=obj.GetComponent<Image>(); image.color=color; image.raycastTarget=false;
            image.sprite=sprite==null?rounded:sprite; image.type=sprite==null?Image.Type.Sliced:Image.Type.Simple;
            PrototypeLevelBuilder.SetRect(image.rectTransform,Vector2.one*.5f,Vector2.one*.5f,position,size);
            return image;
        }
        private Text TextAt(string name,Transform parent,string value,Vector2 position,Vector2 size,int font,Color color)
        {
            var text=PrototypeLevelBuilder.CreateText(name,parent,value,font,TextAnchor.MiddleCenter,color); text.raycastTarget=false;
            PrototypeLevelBuilder.SetRect(text.rectTransform,Vector2.one*.5f,Vector2.one*.5f,position,size); return text;
        }
        private Sprite MakeSprite(int kind)
        {
            const int n=64;
            var texture=new Texture2D(n,n,TextureFormat.RGBA32,false); texture.wrapMode=TextureWrapMode.Clamp;
            for(int y=0;y<n;y++)for(int x=0;x<n;x++)
            {
                float alpha;
                if(kind==0)
                {
                    Vector2 d=new Vector2(Mathf.Max(Mathf.Abs(x-31.5f)-17.5f,0),Mathf.Max(Mathf.Abs(y-31.5f)-17.5f,0));
                    alpha=Mathf.Clamp01(14-d.magnitude);
                }
                else if(kind==1) alpha=Mathf.Clamp01(31-Vector2.Distance(new Vector2(x,y),Vector2.one*31.5f));
                else
                {
                    bool body=x>=10&&x<=53&&y>=4&&y<=34;
                    float distance=Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,41));
                    bool shackle=y>=33 && distance<=19 && (distance>=12 || y<35);
                    bool key=(Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,23))<4) || (x>=30&&x<=33&&y>=12&&y<=23);
                    alpha=(body||shackle)&&!key?1:0;
                }
                texture.SetPixel(x,y,new Color(1,1,1,alpha));
            }
            texture.Apply(); textures.Add(texture);
            return Sprite.Create(texture,new Rect(0,0,n,n),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,kind==0?new Vector4(16,16,16,16):Vector4.zero);
        }
        private void OnDestroy()
        {
            if(puzzle!=null)puzzle.Restarted-=OnRestart;
            foreach(var sprite in new[]{rounded,circle,padlock})if(sprite!=null)Destroy(sprite);
            foreach(var texture in textures)Destroy(texture);
        }
    }
}
