import axios from 'axios';

const mainSystemUrl = import.meta.env.VITE_MAIN_SYSTEM_URL || 
                      (window.location.hostname === 'localhost' ? 'http://localhost:5038' : 'https://system.hoangcn.com');

export interface UserProfileDto {
  isSuccess: boolean;
  data?: {
    userId: string;
    userName: string;
    email: string;
    displayName: string;
    roleName: string;
  };
}

/**
 * Fetch current user profile details from MainSystem using aToken cookie
 */
export const getLoginInfo = async (): Promise<UserProfileDto> => {
  try {
    const response = await axios.get<UserProfileDto>(`${mainSystemUrl}/api/users/me`, {
      withCredentials: true
    });
    return response.data;
  } catch (error) {
    return { isSuccess: false };
  }
};

/**
 * Log out from the MainSystem to clear the HttpOnly aToken cookie
 */
export const logout = async (): Promise<any> => {
  try {
    const response = await axios.post(`${mainSystemUrl}/api/users/logout`, {}, {
      withCredentials: true
    });
    return response.data;
  } catch (error) {
    return { isSuccess: false };
  }
};

import { get as learnMsGet, post as learnMsPost } from './config/axios';

/**
 * Get user profile details specific to LearnMS
 */
export const getLearnMsUserProfile = async () => {
  return await learnMsGet('/api/users/profile');
};

/**
 * Update user profile details specific to LearnMS
 */
export const updateLearnMsUserProfile = async (user: any) => {
  return await learnMsPost('/api/users/profile', user);
};
