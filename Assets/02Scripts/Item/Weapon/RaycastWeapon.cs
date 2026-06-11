using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float Time;
        public Vector3 InitialPosion;
        public Vector3 InitialVelocity;
    }
    public bool IsFiring = false;
    public int FireRate = 25;
    public float BulletSpeed = 1000.0f;
    public float BulletDrop = 0.0f;
    public ParticleSystem[] MuzzleFlashs; // Emission 끈상태
    public ParticleSystem HitEffect;
    public TrailRenderer BulletTracerEffect;

    public Transform RaycastOrigin;
    public Transform RaycastDestination; // 실제 레이 부딪힌 타겟위치(CrossHairTarget)

    private Ray _ray;
    private RaycastHit _hitInfo;
    private float _accumulatedTime;

    private List<Bullet> _bulletList = new List<Bullet>();

    Vector3 GetPosition(Bullet p_bullet)
    {
        // pos + velocity * timt + 0.5 * gravity * time *time = 포물, 중력가속도
        Vector3 gravity = Vector3.down * BulletDrop;
        return (p_bullet.InitialPosion) + (p_bullet.InitialVelocity * p_bullet.Time) + 
            (0.5f * gravity * p_bullet.Time * p_bullet.Time);
    }


    Bullet CreateBullet(Vector3 p_pos, Vector3 p_velocity)
    {
        Bullet bullet = new Bullet();
        bullet.InitialPosion = p_pos;
        bullet.InitialVelocity = p_velocity;
        bullet.Time = 0.0f;
        return bullet;
    }
    public void StartFiring()
    {
        IsFiring = true;
        _accumulatedTime = 0.0f;

    }
    public void UpdateFiring(float p_deltaTime)
    {
        _accumulatedTime += p_deltaTime;
        float fireInterval = 1.0f / FireRate;
        while(_accumulatedTime >= 0.0f)
        {
            FireBullet();
            _accumulatedTime -= fireInterval;
        }
    }
    public void UpdateBullets(float p_delaTime)
    {
        SimulateBullets(p_delaTime);
    }

    private void SimulateBullets(float p_deltaTime)
    {
        _bulletList.ForEach(bullet => 
        {   Vector3 p0 = GetPosition(bullet);
            bullet.Time += p_deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void RaycastSegment(Vector3 p_start, Vector3 p_end, Bullet p_bullet)
    {

    }
    private void FireBullet()
    {
        foreach (var firing in MuzzleFlashs)
        {
            firing.Emit(1);
        }
        /*Vector3 velocity = (RaycastDestination.position - RaycastOrigin.position).normalized;
        var bullet = CreateBullet(RaycastOrigin.position, velocity);
        _bulletList.Add(bullet);*/

        _ray.origin = RaycastOrigin.position;
        _ray.direction = RaycastDestination.position - RaycastOrigin.position;

        var tracer = Instantiate(BulletTracerEffect, _ray.origin, Quaternion.identity);
        tracer.AddPosition(_ray.origin);

        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            //Debug.DrawLine(_ray.origin, _hitInfo.point, Color.red, 1.0f);
            HitEffect.transform.position = _hitInfo.point;
            HitEffect.transform.forward = _hitInfo.normal;
            HitEffect.Emit(1);

            tracer.transform.position = _hitInfo.point;
        }
    }

   

    public void StopFiring()
    {
        IsFiring = false;
    }
}
