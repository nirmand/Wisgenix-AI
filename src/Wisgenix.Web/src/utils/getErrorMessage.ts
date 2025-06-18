interface ApiError {
    response?: {
        data?: {
            message?: string;
        };
    };
    message: string;
}

export const getErrorMessage = (error: ApiError): string => {
    if (error.response?.data?.message) return error.response.data.message;
    return error.message;
};