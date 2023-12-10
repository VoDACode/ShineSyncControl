export class BaseResponse<T> {
    public success: boolean = false;
    public message: string | undefined;
    public data: T | undefined;
}