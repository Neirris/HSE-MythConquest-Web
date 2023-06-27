import React from 'react';
import '../../styles/utilStyles/CustomAlert.css';

const CustomAlert = ({ name, appearanceUrl, itemRarity, ascensionLvl, onClose }) => {
  let stars = '☆'.repeat(itemRarity);

  const oldText = '☆';
  const newText = '★';
  const maxReplacements = ascensionLvl - 1;

  let replacements = 0;
  let index = 0;

  while (
    replacements < maxReplacements &&
    (index = stars.indexOf(oldText, index)) !== -1
  ) {
    stars =
      stars.substring(0, index) +
      newText +
      stars.substring(index + oldText.length);
    index += newText.length;
    replacements++;
  }

  return (
    <div className="custom-alert">
      <img src={appearanceUrl} alt="Character" className="custom-alert-image" />
      <div className="custom-alert-label">{name}</div>
      <div className="custom-alert-label">{stars}</div>
      <button className="custom-alert-close" onClick={onClose}>
        X
      </button>
    </div>
  );
};

export default CustomAlert;