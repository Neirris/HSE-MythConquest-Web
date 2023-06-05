import React, { useEffect, useRef, useState } from 'react';

const ParticleBackground = ({ width, height, top, left }) => {
  const canvasRef = useRef(null);
  // eslint-disable-next-line no-unused-vars
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);

  const particleCount = 200;
  const particleSize = 3;
  const particleSpeed = 1;

  useEffect(() => {
    const canvas = canvasRef.current;
    const context = canvas.getContext('2d');
    let particles = [];

    const resizeCanvas = () => {
      setWindowWidth(window.innerWidth);
      canvas.width = width || window.innerWidth;
      canvas.height = height || window.innerHeight;
    };

    const createParticles = () => {
      for (let i = 0; i < particleCount; i++) {
        const initialX = i < particleCount / 2 ? 0 : canvas.width; // положение
        const initialY = Math.random() * canvas.height;
        const initialVX = i < particleCount / 2 ? Math.random() * particleSpeed : -Math.random() * particleSpeed; // скорость
        const initialVY = Math.random() * particleSpeed - particleSpeed / 2;
        particles.push({
          x: initialX, 
          y: initialY,
          vx: initialVX, 
          vy: initialVY, 
        });
      }
    };
    
    const updateParticles = () => {
      context.clearRect(0, 0, canvas.width, canvas.height);
      
      const wallX1 = canvas.width / 8; 
      const wallX2 = (7 * canvas.width) / 8;
      
      for (let i = 0; i < particles.length; i++) {
        const particle = particles[i];
        
        particle.x += particle.vx;
        particle.y += particle.vy;
        
        // Обновление координат частицы
        if (particle.x < 0 || particle.x > canvas.width) {
          particle.vx *= -1;
        }
        
        if (particle.y < 0 || particle.y > canvas.height) {
          particle.vy *= -1;
        }
        
        // Отскок от стен
        if (Math.abs(particle.x - wallX1) < particleSize || Math.abs(particle.x - wallX2) < particleSize) {
          particle.vx *= -1;
        }
        
        // Отрисовка частицы
        context.beginPath();
        context.arc(particle.x, particle.y, particleSize, 0, Math.PI * 2);
        context.closePath();
        context.fillStyle = '#7289da';
        context.fill();
      }    
      requestAnimationFrame(updateParticles);
    };
  
    resizeCanvas();
    createParticles();
    updateParticles();

    window.addEventListener('resize', resizeCanvas);

    return () => {
      window.removeEventListener('resize', resizeCanvas);
    };
  }, [width, height]);

  const canvasStyle = {
    position: 'fixed',
    top: top || 0,
    left: left || 0,
    right: left === undefined ? 0 : 'auto',
    zIndex: -1,
  };

  return <canvas ref={canvasRef} style={canvasStyle} />;
};

export default ParticleBackground;

