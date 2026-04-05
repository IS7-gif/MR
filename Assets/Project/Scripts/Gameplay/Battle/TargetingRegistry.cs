using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle
{
    public class TargetingRegistry
    {
        private readonly List<ITargetable> _units = new();


        public void Register(ITargetable unit)
        {
            _units.Add(unit);
        }

        public void Unregister(ITargetable unit)
        {
            _units.Remove(unit);
        }

        public ITargetable FindAtPosition(Vector2 screenPos, Camera cam, float offsetPx = 20f)
        {
            var z = Mathf.Abs(cam.transform.position.z);
            var worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
            worldPos.z = 0f;

            var offsetWorld = offsetPx / Screen.height * (cam.orthographicSize * 2f);

            for (var i = 0; i < _units.Count; i++)
            {
                var unit = _units[i];
                var expanded = new Bounds(
                    unit.WorldBounds.center,
                    unit.WorldBounds.size + Vector3.one * offsetWorld * 2f);

                if (expanded.Contains(worldPos))
                    return unit;
            }

            return null;
        }

        public void ClearAll()
        {
            for (var i = 0; i < _units.Count; i++)
            {
                _units[i].SetSourceHighlight(false);
                _units[i].SetTargetHighlight(false, default);
            }
        }
    }
}