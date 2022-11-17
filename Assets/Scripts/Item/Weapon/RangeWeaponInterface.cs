using System.Collections;
using UnityEngine;

namespace Item.Weapon
{
    public class RangeWeaponInterface : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _firePoint;        
        [SerializeField] private RangeWeapon _rangeWeapon;

        private float _lastTimeAttack;

        private void Start()
        {
            _spriteRenderer.sprite = _rangeWeapon.Sprite;
            PlayerShoot.shootInput += Shoot;
        }

        public Item GetItem()
        {
            return (Item)_rangeWeapon;
        }

        public void Shoot(Vector2 pointToShoot)
        {
            if (_rangeWeapon.IsReloading)
            {
                return;
            }

            if (Time.time < _lastTimeAttack + _rangeWeapon.AttackRate)
            {
                return;
            }

            if (_rangeWeapon.CurrentAmmo == 0)
            {
                StartCoroutine(Reload());
                return;
            }

            _rangeWeapon.CurrentAmmo--;

            for (int bullet = 0; bullet < _rangeWeapon.NumberOfBulletsPerShot; bullet++)
            {
                Vector2 firePointPosition = _firePoint.position;
                Vector2 directionToShoot = pointToShoot - firePointPosition;
                float turn = Random.Range(-_rangeWeapon.SpreadDegrees, _rangeWeapon.SpreadDegrees) * Mathf.Deg2Rad;

                // Applying a spread to the bullet using polar coordinate system 
                float angleDir = Mathf.Atan2(directionToShoot.y, directionToShoot.x) + turn;
                directionToShoot = new Vector2(Mathf.Cos(angleDir), Mathf.Sin(angleDir));
                
                // TODO: fix shooting towards shooter

                RaycastHit2D hitInfo = Physics2D.Raycast(firePointPosition, directionToShoot);

                Debug.DrawRay(firePointPosition, directionToShoot, Color.black, 10f);

                if (hitInfo)
                {
                    Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        enemy.TakeDamage(_rangeWeapon.AttackDamage);
                    }
                }
                _lastTimeAttack = Time.time;
            }
        }

        IEnumerator Reload()
        {
            _rangeWeapon.IsReloading = true;

            yield return new WaitForSeconds(_rangeWeapon.ReloadTime);
            _rangeWeapon.CurrentAmmo = _rangeWeapon.MaxAmmo;

            _rangeWeapon.IsReloading = false;
        }
    }
}