import React, { useState, useEffect } from 'react';
import { Route, Routes, Navigate, Link, useLocation } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/utils/Layout';
import './styles/utilStyles/Background.css';
import axios from 'axios';

export default function App() {
  const location = useLocation();
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);
  axios.defaults.headers.common['Cookie'] = document.cookie;

  useEffect(() => {
    const isAuth = checkAuthStatus();
    setIsAuthenticated(isAuth);
    setLoading(false);
  }, []);

  const checkAuthStatus = () => {
    const cookies = decodeURIComponent(document.cookie);
    if (cookies && cookies.includes('User')) {
      const userCookie = cookies.split('; ').find(cookie => cookie.startsWith('User='));
      if (userCookie) {
        const decodedUser = JSON.parse(userCookie.split('=')[1]);
        return decodedUser.isAuth;
      }
    }
    return false;
  };

  if (loading) {
    return <div>Загрузка...</div>;
  }

  if (!isAuthenticated && location.pathname !== '/auth') {
    return <Navigate to="/auth" replace />;
  }

  // if (location.pathname === '/logout') {
  //   return <Navigate to="/auth"/>;
  // }

  return (
    <Layout>
      <Routes>
        {AppRoutes.map((route, index) => {
          const { element, ...rest } = route;
          return <Route key={index} {...rest} element={element} />;
        })}
      </Routes>
    </Layout>
  );
}