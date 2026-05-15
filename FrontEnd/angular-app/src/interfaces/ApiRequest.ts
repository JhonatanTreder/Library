import { HttpMethodType } from "../types/HttpMethodType";

export interface ApiRequest {
    url: string;
    httpMethod: HttpMethodType;
}