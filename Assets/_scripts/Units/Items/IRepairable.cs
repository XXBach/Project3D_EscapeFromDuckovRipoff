using UnityEngine;

public interface IRepairable
{
    public void TakeDurabilityDamage(float damage);
    public void Repair(float repairAmount);
}
