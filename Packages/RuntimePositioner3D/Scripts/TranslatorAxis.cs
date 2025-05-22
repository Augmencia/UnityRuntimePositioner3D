using System.Linq;
using UnityEngine;

namespace Augmencia.RuntimePositioner3D
{
    public class TranslatorAxis : AxisBase
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private MeshRenderer[] _meshRenderers;
        [SerializeField] private Vector3 _axis;
        internal Vector3 Axis => _axis;

        private Positioner3D _positioner;
        Vector3 _offset;
        private int _axisIndex = 0;
        private Material _originalMaterial;

        internal override void Initialize(Positioner3D positioner)
        {
            _positioner = positioner;

            for (int i = 0; i < 3; ++i)
            {
                if (Axis[i] != 0)
                {
                    _axisIndex = i;
                    break;
                }
            }

            _originalMaterial = _meshRenderers[0].material;
        }

        internal override void SetMaterial(Material material)
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                meshRenderer.material = material;
            }
        }

        internal override void ResetMaterial()
        {
            SetMaterial(_originalMaterial);
        }

        internal override bool TryStartDragging(RaycastHit hit)
        {
            if (_collider == hit.collider)
            {
                Ray screenRay = new Ray(_positioner.Camera.transform.position, (hit.point - _positioner.Camera.transform.position).normalized);
                Plane plane = new Plane((_positioner.Camera.transform.position - _positioner.transform.position).normalized, _positioner.transform.position);
                if (plane.Raycast(screenRay, out float distance))
                {
                    Vector3 point = screenRay.GetPoint(distance);
                    _offset = _positioner.transform.position - point;
                    return true;
                }
            }
            return false;
        }

        internal override void Drag(Vector3 screenPoint)
        {
            Ray screenRay = _positioner.Camera.ScreenPointToRay(screenPoint);
            Plane plane = new Plane((_positioner.Camera.transform.position - _positioner.transform.position).normalized, _positioner.transform.position);
            if (plane.Raycast(screenRay, out float distance))
            {
                Vector3 point = screenRay.GetPoint(distance) + _offset;
                Vector3 pos = Vector3.zero;
                pos[_axisIndex] = _positioner.transform.InverseTransformPoint(point)[_axisIndex];
                _positioner.ManipulatedObject.position = _positioner.transform.TransformPoint(pos);
            }
        }
    }
}