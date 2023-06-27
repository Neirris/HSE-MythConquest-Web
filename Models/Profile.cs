namespace MythConquestWeb
{
    public class ProfileInfo
    {
        public string nickname { get; set; }
        public string lvl { get; set; }
        public string user_pfp { get; set; }
        public string exp { get; set; }
        public string exp_req { get; set; }
        public string wins { get; set; }
        public string loses { get; set; }

        public string coins { get; set; }

    }

    public class ProfileStats
    {
        public string char_id { get; set; }
        public string name { get; set; }
        public string appearance_url { get; set; }
        public string char_class { get; set; }
        public string rarity { get; set; }
        public string gender { get; set; }
        public string value { get; set; }
        public string health_points { get; set; }

        public string defense { get; set; }

        public string attack { get; set; }

        public string crit { get; set; }

        public string initiative { get; set; }

        public string evasion { get; set; }

    }

    public class TowerHp
    {

        public string hp_potions { get; set; }
    }

    public class TicketPrice
    {

        public string ticket_price { get; set; }
    }

    public class CurrentCoins
    {

        public string coins { get; set; }
        public string curr_ticket { get; set; }
    }

    public class CurrentEquipmentLvl
    {
        public string lvl_weapon { get; set; }
        public string lvl_armor { get; set; }
        public string lvl_greaves { get; set; }
        public string lvl_boots { get; set; }
        public string lvl_gloves { get; set; }
        public string lvl_jewelry { get; set; }

    }

    public class EquipmentPrice
    {
        public string weapon_price { get; set; }
        public string armor_price { get; set; }
        public string greaves_price { get; set; }
        public string boots_price { get; set; }
        public string gloves_price { get; set; }
        public string jewelry_price { get; set; }

    }

    public class User
    {
        public int userId { get; set; }
        public string userName { get; set; }

        public string userPass { get; set; }

        public bool isAuth { get; set; }

        public bool isAdmin { get; set; }

        public User()
        {
            userId = -1;
            userName = "??";
            userPass = "??";
            isAuth = false;
            isAdmin = false;
        }
    }

    public class DropResult
    {
        public string name { get; set; }
        public string appearance_url { get; set; }
        public string itemRarity { get; set; }
        public string ascension_lvl { get; set; }
    }


}

