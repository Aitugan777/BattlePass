using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Models
{
    public class AitukTask
    {
        public string Name { get; set; }

        public ETaskType TaskType { get; set; }

        public ushort EquipmentItemID { get; set; }

        public ushort TargetItemID { get; set; }

        public float ActionCoefficient { get; set; }

        public int MaxCount { get; set; }
    }

    public enum ETaskType
    {
        KILL_ZOMBIE, //+ Убить обычного зомби
        KILL_ZOMBIE_MEGA, //+ Убить мега зомби
        KILL_PLAYER, //+ Убить игрока
        KILL_PLAYER_HEAD, //+ Убить игрока в голову
        KILL_PLAYER_PUNCH, //+ Убить игрока рукой
        KILL_PLAYER_ROADKILL, //+ Убить игрока на транспорте
        KILL_ANIMAL, //+ Убить животное
        DAMAGE_PLAYER, //+ Нанести урон игроку (считается не урон, а количество нанесения)
        DAMAGE_ZOMBIE, //+ нанести урон по зомби (считается не урон, а количество нанесения)
        DAMAGE_VEHICLE, //+ Нанести урон по транспорту (считается не урон, а количество нанесения)
        DAMAGE_STRUCTURE, //+ Нанести урон по структуре (считается не урон, а количество нанесения)
        DAMAGE_BARRICADE, //+ Нанести урон по баррикаде (считается не урон, а количество нанесения)
        REPAIR_VEHICLE, //+
        REPAIR_STRUCTURE, //+
        REPAIR_BARRICADE, //+
        DESTROY_STRUCTURE, //+
        DESTROY_BARRICADE, //+
        TRAVEL_FOOT, //+ //Дистанция считается за каждый метр
        TRAVEL_VEHICLE, //+ //Дистанция считается за каждый метр
        TRAVEL_FLY, //+ //Дистанция считается за каждый метр
        FOUND_FISH, //+ Поймать рыб
        FOUND_PLANTS, //+ Вырастить траву
        TIME_PLAYED, //+ Сыграно время на сервере (считается каждую минуту)
        FOUND_EXPERIENCE, //+ Собрано опыта, считается не сколько собрал, а каждый момент собрания опыта
        DIE_ACID, //+ Умереть от кислоты
        DIE_BOOM, //+ Умереть от взрыва
        DIE_OXYGEN, //+ Умереть от нехватки воздуха
        DIE_PLAYER, //+ Умереть от игрока
        DIE_VIRUS, //+ Умереть от инфекции
        DIE_ZOMBIE, //+ Умереть от зомби
        DIE_ANIMAL, //+ Умереть от животных
        CHAT_WRITE, //+ написать в чат что-то
        REVIVE, //+ Возродится
        KILL_ZOMBIE_CRAWLER, //+
        KILL_ZOMBIE_SPRINTER, //+
        KILL_ZOMBIE_FLANKER_FRIENDLY, //+
        KILL_ZOMBIE_FLANKER_STALK, //+
        KILL_ZOMBIE_BURNER, //+
        KILL_ZOMBIE_ACID, //+
        KILL_ZOMBIE_BOSS_ELECTRIC, //+
        KILL_ZOMBIE_BOSS_WIND, //+
        KILL_ZOMBIE_BOSS_FIRE, //+
        KILL_ZOMBIE_BOSS_ALL, //+
        KILL_ZOMBIE_BOSS_MAGMA, //+
        KILL_ZOMBIE_SPIRIT, //+
        KILL_ZOMBIE_BOSS_SPIRIT, //+
        KILL_ZOMBIE_BOSS_NUCLEAR, //+
        KILL_ZOMBIE_DL_RED_VOLATILE, //+
        KILL_ZOMBIE_DL_BLUE_VOLATILE, //+
        KILL_ZOMBIE_BOSS_ELVER_STOMPER, //+
        KILL_ZOMBIE_BOSS_KUWAIT //+
    }
}
