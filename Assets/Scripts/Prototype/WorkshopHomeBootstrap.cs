using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    public sealed class WorkshopHomeBootstrap : MonoBehaviour
    {
        public static bool CategoriesOnArrival;
        private static bool splashShown;
        private RectTransform layout, page;
        private RawImage backdrop;
        private CanvasGroup splash;
        public const float SplashDuration = 4f;
        private string currentPage;
        private Text musicLabel, soundLabel;
        private Sprite rounded;
        private Texture2D roundedTexture;
        private static readonly Color Ink = new Color(.25f,.20f,.15f);
        private static readonly Color Cream = new Color(.98f,.95f,.87f);
        private static readonly Color Teal = new Color(.13f,.37f,.34f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { splashShown=false; CategoriesOnArrival=false; }

        private void Awake()
        {
            var camera = new GameObject("Home Camera",typeof(Camera),typeof(AudioListener)).GetComponent<Camera>();
            camera.backgroundColor = Color.black; camera.clearFlags=CameraClearFlags.SolidColor;
            var canvas = new GameObject("Home UI",typeof(Canvas),typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            backdrop=new GameObject("Home Backdrop",typeof(RectTransform),typeof(RawImage)).GetComponent<RawImage>();
            backdrop.transform.SetParent(canvas.transform,false);
            backdrop.texture=Resources.Load<Texture2D>("Art/Workshop/Toy-Shop-Background");
            backdrop.raycastTarget=false;
            Stretch(backdrop.rectTransform);
            var wash=Box("Home Wash",canvas.transform,Vector2.zero,Vector2.zero,new Color(.25f,.18f,.12f,.32f));
            Stretch(wash.rectTransform);
            layout=new GameObject("Home Layout",typeof(RectTransform)).GetComponent<RectTransform>();
            layout.SetParent(canvas.transform,false); layout.sizeDelta=new Vector2(1080,1500);
            if(FindFirstObjectByType<EventSystem>()==null) new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
            MakeRounded();
            if(CategoriesOnArrival) { CategoriesOnArrival=false; ShowCategories(); } else ShowHome();
            if(!splashShown) { splashShown=true; StartCoroutine(ShowSplash(canvas.transform)); }
            else WorkshopMusic.EnsurePlaying();
            RefreshLayout(Screen.safeArea);
        }

        private IEnumerator ShowSplash(Transform canvas)
        {
            var cover=Box("Lunyx Splash",canvas,Vector2.zero,Vector2.zero,Color.black);
            Stretch(cover.rectTransform); cover.sprite=null; cover.type=Image.Type.Simple; cover.raycastTarget=true;
            splash=cover.gameObject.AddComponent<CanvasGroup>();
            var controls=layout.gameObject.AddComponent<CanvasGroup>();
            controls.interactable=false; controls.blocksRaycasts=false;
            var logo=new GameObject("Lunyx Logo",typeof(RectTransform),typeof(RawImage)).GetComponent<RawImage>();
            logo.transform.SetParent(cover.transform,false);
            logo.texture=Resources.Load<Texture2D>("Art/Workshop/Lunyx-Logo"); logo.raycastTarget=false;
            var fit=logo.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent; fit.aspectRatio=4f/3f;
            var logoGroup=logo.gameObject.AddComponent<CanvasGroup>(); logoGroup.alpha=0;
            // The supplied logo is displayed intact with its original black background.
            float elapsed=0;
            yield return null; // Discard the first scene-loading frame before timing the visible logo.
            while(elapsed<SplashDuration)
            {
                if(Application.isFocused || Application.isBatchMode) elapsed+=Mathf.Min(Time.unscaledDeltaTime,.1f);
                logoGroup.alpha=Mathf.Clamp01(elapsed/.55f);
                if(elapsed>SplashDuration-.6f) splash.alpha=Mathf.Clamp01((SplashDuration-elapsed)/.6f);
                yield return null;
            }
            Destroy(cover.gameObject); splash=null;
            controls.interactable=true; controls.blocksRaycasts=true;
            WorkshopMusic.EnsurePlaying();
        }

        private void BeginPage(string name,string title,string subtitle)
        {
            currentPage=name; musicLabel=soundLabel=null;
            if(page!=null) { page.gameObject.SetActive(false); Destroy(page.gameObject); }
            page=new GameObject(name+" Page",typeof(RectTransform)).GetComponent<RectTransform>();
            page.SetParent(layout,false); page.sizeDelta=layout.sizeDelta;
            Box("Page Card",page,Vector2.zero,new Vector2(920,1300),Cream);
            Label("Studio",page,"LUNYX  INTERACTIVE STUDIOS",new Vector2(0,550),new Vector2(820,55),25,new Color(.49f,.35f,.16f));
            Label("Page Title",page,title,new Vector2(0,440),new Vector2(820,140),48,Ink);
            Label("Page Subtitle",page,subtitle,new Vector2(0,325),new Vector2(790,100),28,Ink);
        }
        public void ShowHome()
        {
            BeginPage("Home","PUZZLE COLLECTIONS",string.IsNullOrEmpty(WorkshopPlayerProfile.Nickname)
                ? "Welcome! Make a little time to play." : "Welcome back, "+WorkshopPlayerProfile.Nickname+"!");
            ButtonAt("Home Play","PLAY",140,ShowCategories,Teal);
            string board=WorkshopPlayerProfile.ContinueBoard;
            var resume=ButtonAt("Home Continue",board==null?"NO PUZZLE IN PROGRESS":"CONTINUE  •  "+WorkshopPlayerProfile.BoardName(board),0,
                WorkshopPlayerProfile.Continue,new Color(.62f,.37f,.18f));
            resume.interactable=board!=null;
            ButtonAt("Open Settings","SETTINGS",-160,ShowSettings,new Color(.32f,.38f,.39f));
            Label("Home Note",page,"A little curiosity. A piece at a time.",new Vector2(0,-490),new Vector2(780,90),27,Ink);
        }
        public void ShowCategories()
        {
            BeginPage("Categories","CHOOSE A COLLECTION","Every collection is a new place to explore.");
            var image=new GameObject("Toy Shop Category Art",typeof(RectTransform),typeof(RawImage)).GetComponent<RawImage>();
            image.transform.SetParent(page,false); image.texture=Resources.Load<Texture2D>("Art/Workshop/Toy-Shop-Background");
            PrototypeLevelBuilder.SetRect(image.rectTransform,Vector2.one*.5f,Vector2.one*.5f,new Vector2(0,90),new Vector2(720,310));
            image.raycastTarget=false;
            if(image.texture!=null) { float crop=(310f/720f)*image.texture.width/image.texture.height; image.uvRect=new Rect(0,(1-crop)*.5f,1,crop); }
            ButtonAt("Open Toy Shop","TOY SHOP  •  " + ThreeDBoardProgress.CollectionStatus,-150,ThreeDBoardNavigation.OpenSelector,Teal);
            Label("Future Collections",page,ThreeDBoardProgress.CompletedCount==WorkshopPlayerProfile.Boards.Length ? "Collection complete! Replay a toy for a fresh challenge." : "More collections will join the shelves.",new Vector2(0,-310),new Vector2(780,80),27,Ink);
            ButtonAt("Categories Home","BACK TO HOME",-490,ShowHome,new Color(.42f,.36f,.29f));
        }
        public void ShowSettings()
        {
            BeginPage("Settings","SETTINGS","Make yourself comfortable.");
            soundLabel=ButtonAt("Settings Sound","",140,()=> { FeedbackAudio.ToggleSound(); RefreshAudio(); },Teal).GetComponentInChildren<Text>();
            musicLabel=ButtonAt("Settings Music","",0,()=> { WorkshopMusic.Toggle(); RefreshAudio(); },Teal).GetComponentInChildren<Text>();
            ButtonAt("Open Profile","PLAYER NAME",-140,ShowProfile,new Color(.42f,.34f,.51f));
            ButtonAt("How To Play","HOW TO PLAY",-280,ShowGuide,new Color(.42f,.34f,.51f));
            ButtonAt("Settings Home","BACK TO HOME",-450,ShowHome,new Color(.42f,.36f,.29f));
            RefreshAudio();
        }
        private void ShowGuide()
        {
            BeginPage("Guide","HOW TO PLAY","Turn it. Match it. Take it apart.");
            Label("Guide Text",page,"Drag sideways or up and down to explore the toy.\n\nTap bright screws that match an open tray.\nCollect three of one color to clear that tray.\n\nRemove every screw in an assembly to release it.\nA padlock means another part must come off first.\n\nEvery standard puzzle can be solved with two free trays.\nReset View changes the angle; Restart resets the puzzle.",
                new Vector2(0,-20),new Vector2(760,650),29,Ink);
            ButtonAt("Guide Settings","BACK TO SETTINGS",-490,ShowSettings,new Color(.42f,.36f,.29f));
        }
        public void ShowProfile()
        {
            BeginPage("Profile","YOUR PLAYER NAME","Choose an optional nickname.");
            var box=Box("Nickname Input",page,new Vector2(0,120),new Vector2(760,110),Color.white);
            box.raycastTarget=true;
            var input=box.gameObject.AddComponent<InputField>();
            var text=Label("Nickname Text",box.transform,"",Vector2.zero,new Vector2(700,95),36,Ink);
            text.alignment=TextAnchor.MiddleLeft;
            var placeholder=Label("Nickname Placeholder",box.transform,"Your nickname",Vector2.zero,new Vector2(700,95),32,new Color(.5f,.48f,.43f));
            placeholder.alignment=TextAnchor.MiddleLeft;
            input.textComponent=text; input.placeholder=placeholder; input.characterLimit=20;
            input.lineType=InputField.LineType.SingleLine; input.text=WorkshopPlayerProfile.Nickname;
            Label("Local Profile Note",page,"Saved on this device only.\nNo account, email, or password needed.\nLeave it blank to play without a name.",new Vector2(0,-60),new Vector2(780,190),29,Ink);
            ButtonAt("Save Nickname","SAVE NAME",-280,()=> { WorkshopPlayerProfile.SaveName(input.text); ShowSettings(); },Teal);
            ButtonAt("Cancel Nickname","CANCEL",-440,ShowSettings,new Color(.42f,.36f,.29f));
        }
        private void RefreshAudio()
        {
            if(soundLabel!=null) soundLabel.text="SOUND EFFECTS: "+(FeedbackAudio.IsSoundEnabled?"ON":"OFF");
            if(musicLabel!=null) musicLabel.text="MUSIC: "+(WorkshopMusic.IsEnabled?"ON":"OFF");
        }
        public void RefreshLayout(Rect safe)
        {
            layout.position=safe.center;
            layout.localScale=Vector3.one*Mathf.Min(safe.width/1080f,safe.height/1500f);
            if(backdrop.texture!=null)
            {
                float screenAspect=(float)Screen.width/Mathf.Max(1,Screen.height);
                float artAspect=(float)backdrop.texture.width/backdrop.texture.height;
                backdrop.uvRect=screenAspect>artAspect
                    ? new Rect(0,(1-artAspect/screenAspect)*.5f,1,artAspect/screenAspect)
                    : new Rect((1-screenAspect/artAspect)*.5f,0,screenAspect/artAspect,1);
            }
        }
        private void Update()
        {
            RefreshLayout(Screen.safeArea);
            if(!Input.GetKeyDown(KeyCode.Escape)) return;
            if(splash!=null) return;
            if(currentPage=="Guide" || currentPage=="Profile") ShowSettings();
            else if(currentPage!="Home") ShowHome();
        }
        private Button ButtonAt(string name,string caption,float y,UnityEngine.Events.UnityAction action,Color color)
        {
            var image=Box(name,page,new Vector2(0,y),new Vector2(760,106),color); image.raycastTarget=true;
            var button=image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(action);
            Label(name+" Label",image.transform,caption,Vector2.zero,new Vector2(720,96),31,Cream);
            return button;
        }
        private Image Box(string name,Transform parent,Vector2 position,Vector2 size,Color color)
        {
            var obj=new GameObject(name,typeof(RectTransform),typeof(Image)); obj.transform.SetParent(parent,false);
            var image=obj.GetComponent<Image>(); image.color=color; image.raycastTarget=false;
            image.sprite=rounded; if(rounded!=null) image.type=Image.Type.Sliced;
            PrototypeLevelBuilder.SetRect(image.rectTransform,Vector2.one*.5f,Vector2.one*.5f,position,size);
            return image;
        }
        private Text Label(string name,Transform parent,string value,Vector2 position,Vector2 size,int font,Color color)
        {
            var text=PrototypeLevelBuilder.CreateText(name,parent,value,font,TextAnchor.MiddleCenter,color);
            text.supportRichText=false; text.raycastTarget=false;
            PrototypeLevelBuilder.SetRect(text.rectTransform,Vector2.one*.5f,Vector2.one*.5f,position,size);
            return text;
        }
        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.offsetMin=rect.offsetMax=Vector2.zero;
        }
        private void MakeRounded()
        {
            roundedTexture=new Texture2D(64,64,TextureFormat.RGBA32,false);
            roundedTexture.wrapMode=TextureWrapMode.Clamp;
            for(int y=0;y<64;y++)for(int x=0;x<64;x++)
            {
                var d=new Vector2(Mathf.Max(Mathf.Abs(x-31.5f)-16,0),Mathf.Max(Mathf.Abs(y-31.5f)-16,0));
                roundedTexture.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(15.5f-d.magnitude)));
            }
            roundedTexture.Apply();
            rounded=Sprite.Create(roundedTexture,new Rect(0,0,64,64),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(20,20,20,20));
        }
        private void OnDestroy() { if(rounded!=null)Destroy(rounded); if(roundedTexture!=null)Destroy(roundedTexture); }
    }
}
