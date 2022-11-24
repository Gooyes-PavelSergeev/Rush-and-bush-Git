using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : PlayerComponent, IHitDetector
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private HurtboxMask _hurtboxMask = HurtboxMask.Enemy;
    [SerializeField] private GameObject _owner;

    private readonly float m_thickness = 0.025f;
    private IHitResponder m_hitResponder;
    public IHitResponder HitResponder { get => m_hitResponder; set => m_hitResponder = value; }

    public void CheckHit()
    {
        Vector3 scaledSize = new Vector3(
            _collider.size.x * transform.lossyScale.x,
            _collider.size.y * transform.lossyScale.y,
            _collider.size.z * transform.lossyScale.z);

        float distance = scaledSize.z - m_thickness;
        Vector3 center = transform.TransformPoint(_collider.center);
        Vector3 direction = _collider.transform.forward;
        Vector3 start = _collider.transform.position;
        Vector3 halfExtents = new Vector3(scaledSize.x, m_thickness, scaledSize.z) / 2;
        Quaternion rotation = Quaternion.identity;


        HitData hitData;
        IHurtbox hurtBox;
        RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, rotation, distance, _layerMask);
        foreach (RaycastHit hit in hits)
        {
            hurtBox = hit.collider.GetComponent<IHurtbox>();
            if (hurtBox != null)
                if (hurtBox.Active)
                    if (hurtBox.Owner != _owner)
                        if (_hurtboxMask.HasFlag((HurtboxMask)hurtBox.Type))
                        {
                            hitData = new HitData
                            {
                                hitConfig = m_hitResponder == null ? null : m_hitResponder.HitConfig,
                                hitPoint = hit.point == Vector3.zero ? center : hit.point,
                                hitNormal = hit.normal,
                                hurtbox = hurtBox,
                                hitDetector = this
                            };

                            if (hitData.Validate())
                            {
                                _entity.PlayerData.RegisterHit(hitData);
                                hitData.hitDetector.HitResponder?.Response(hitData);
                            }

                        }
        }
    }
}
