export class base64 {
    public static encode(str: string): string {
        const codeUnits = new Uint16Array(str.length);
        for (let i = 0; i < codeUnits.length; i++) {
            codeUnits[i] = str.charCodeAt(i);
        }
        return btoa(String.fromCharCode(...new Uint8Array(codeUnits)));
    }

    public static decode(str: string): string {
        const binary = atob(str);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < bytes.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }
        return String.fromCharCode(...new Uint16Array(bytes.buffer));
    }
}