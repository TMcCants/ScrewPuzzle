using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Camera-mounted artwork, behind the toy, with aspect-fill cropping and no input surface.</summary>
    public sealed class WorkshopAirportBackdrop : MonoBehaviour
    {
        private Camera view;
        private Transform backdrop;
        private Material material;
        private Texture2D artwork;

        private void Awake()
        {
            view = GetComponent<Camera>();
        }

        public void Configure(string resource, string title)
        {
            artwork = Resources.Load<Texture2D>(resource);
            var shader = Resources.Load<Shader>("Art/Workshop/AirportBackdrop");
            if (artwork == null || shader == null) { enabled = false; return; }
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = title;
            quad.GetComponent<Collider>().enabled = false;
            Destroy(quad.GetComponent<Collider>());
            backdrop = quad.transform;
            backdrop.SetParent(transform, false);
            backdrop.localPosition = new Vector3(0, 0, 50);
            material = new Material(shader);
            material.mainTexture = artwork;
            quad.GetComponent<Renderer>().sharedMaterial = material;
            OnPreCull();
        }

        private void OnPreCull()
        {
            if (backdrop == null) return;
            float height = 100f * Mathf.Tan(view.fieldOfView * Mathf.Deg2Rad * .5f);
            float aspect = (float)artwork.width / artwork.height;
            float width = Mathf.Max(height * aspect, height * view.aspect);
            backdrop.localScale = new Vector3(width, width / aspect, 1);
        }

        private void OnDestroy() { if (material != null) Destroy(material); }
    }
}
