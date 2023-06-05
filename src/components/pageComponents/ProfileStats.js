import React from 'react';


const ProfileStats = ({className, rarity, gender, value, attack, crit, initiative, evasion, health, defense }) => {
  return (
    <div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Класс</span>
          <span style={{marginTop: "5px"}}>{className}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Редкость</span>
          <span style={{marginTop: "5px"}}>{rarity}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Пол</span>
          <span style={{marginTop: "5px"}}>{gender}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Ценность</span>
          <span style={{marginTop: "5px"}}>{value}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Атака</span>
          <span style={{marginTop: "5px"}}>{attack}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Крит</span>
          <span style={{marginTop: "5px"}}>{crit}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Инициатива</span>
          <span style={{marginTop: "5px"}}>{initiative}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Уклонение</span>
          <span style={{marginTop: "5px"}}>{evasion}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Здоровье</span>
          <span style={{marginTop: "5px"}}>{health}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Защита</span>
          <span style={{marginTop: "5px"}}>{defense}</span>
        </div>
      </div>
    </div>
  );
};

export default ProfileStats;
