using UnityEngine;
using System.Linq;

public static class PriorityTargetSystem
{
    public static Transform GetPriorityTarget(Transform seeker, Building.BuildingType[] priorityOrder)
    {
        var allBuildings = GameObject.FindObjectsOfType<Building>()
            .Where(b => b.health > 0);

        // Сначала ищем цели по приоритету
        foreach (var buildingType in priorityOrder)
        {
            var target = allBuildings
                .Where(b => b.type == buildingType)
                .OrderBy(b => Vector3.Distance(seeker.position, b.transform.position))
                .FirstOrDefault();

            if (target != null)
            {
                // Для заборов - дополнительная проверка
                if (target.type == Building.BuildingType.Fence && !target.isBlockingPath)
                    continue;

                return target.transform;
            }
        }

        return null;
    }
}