import React from 'react';
import NickIcon from '../../images/icons/change.png';


const ProfileInfo = ({ nickName, lvl, exp, expReq, money, valueSum, pvpWin, pvpLose, towerLvl }) => {
  return (
    <div >
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Никнейм</span>
          <span style={{marginTop: "5px"}}>{nickName}</span>
          {/* <img className="nick-change-icon" src={NickIcon} alt=''></img>  */}
        </div>
        <div className="text-block">
          <span className="text-bold">Уровень</span>
          <span style={{marginTop: "5px"}}>{lvl}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Опыт</span>
          <span style={{marginTop: "5px"}}>{exp}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Опыта до повышения</span>
          <span style={{marginTop: "5px"}}>{expReq}</span>
        </div>
      </div>
      <div className="row-c">
        <div className="text-block">
          <span className="text-bold">Монеты</span>
          <span style={{marginTop: "5px"}}>{money}</span>
        </div>
        <div className="text-block">
          <span className="text-bold">Победы/Поражения (PvP)</span>
          <span style={{marginTop: "5px"}}>{pvpWin}/{pvpLose}</span>
        </div>
        {/* <div className="text-block">
          <span className="text-bold">Ценность</span>
          <span style={{marginTop: "5px"}}>{valueSum}</span>
        </div> */}
      </div>
      <div className="row-c">
        {/* <div className="text-block">
          <span className="text-bold">Победы/Поражения (PvP)</span>
          <span style={{marginTop: "5px"}}>{pvpWin}/{pvpLose}</span>
        </div> */}
        {/* <div className="text-block">
          <span className="text-bold">Этаж</span>
          <span style={{marginTop: "5px"}}>{towerLvl}</span>
        </div> */}
      </div>
    </div>
  );
};

export default ProfileInfo;
