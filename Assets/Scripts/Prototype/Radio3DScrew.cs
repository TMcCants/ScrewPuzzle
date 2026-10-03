using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>A 3D screw keeps its original mount so Restart can restore it after tray movement.</summary>
    public sealed class Radio3DScrew : MonoBehaviour
    {
        public ScrewColorId ColorId { get; private set; }
        public bool IsRemoved { get; private set; }
        public bool IsInnerLayer => coveringPlate != null;
        internal string SaveSignature => name + ":" + (int)ColorId + ":" + (coveringPlate == null ? "" : coveringPlate.name);
        public bool IsAccessible { get { return coveringPlate == null || (coveringPlate.IsReleased && !coveringPlate.IsAnimating); } }
        private Radio3DPlate coveringPlate;
        private Transform mount;
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;
        private Collider[] colliders;

        public void Initialize(ScrewColorId color, Radio3DPlate cover = null)
        {
            ColorId = color;
            coveringPlate = cover;
            mount = transform.parent;
            position = transform.localPosition;
            rotation = transform.localRotation;
            scale = transform.localScale;
            colliders = GetComponentsInChildren<Collider>();
        }

        public void Detach()
        {
            IsRemoved = true;
            foreach (Collider collider in colliders) collider.enabled = false;
            transform.SetParent(null, true);
        }

        public void Restore()
        {
            transform.SetParent(mount, false);
            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;
            foreach (Collider collider in colliders) collider.enabled = true;
            IsRemoved = false;
            gameObject.SetActive(true);
        }
    }
}
