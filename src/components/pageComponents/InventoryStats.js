import React from 'react';

const InventoryStats = ({ name, className, rarity, gender, value, attack, crit, initiative, evasion, health, defense }) => {
  return (
    <div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Имя</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{name}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Класс</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{className}</span>
        </div>
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Редкость</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{rarity}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Пол</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{gender}</span>
        </div>
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Ценность</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{value}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Атака</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{attack}</span>
        </div>
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Крит</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{crit}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Инициатива</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{initiative}</span>
        </div>
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Уклонение</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{evasion}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Здоровье</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{health}</span>
        </div>
        <div className="text-block">
          <span className="text-bold" style={{fontSize: "30px"}}>Защита</span>
          <span style={{marginTop: "5px", fontSize: "30px"}}>{defense}</span>
        </div>
      </div>
    </div>
  );
};

export default InventoryStats;
