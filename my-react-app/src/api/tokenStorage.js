
let accessToken = null;
let refreshToken = null;

export const setAuthTokens = (access, refresh) => {
    accessToken = access;
    refreshToken = refresh;
};

export const getAccessToken = () => accessToken;
export const getRefreshToken = () => refreshToken;

export const clearAuthTokens = () => {
    accessToken = null;
    refreshToken = null;
};
