export abstract class BaseValueModel{
    public value: string = "";
    public type: number = 0;

    public static setToDefaultValueIfEmpty(model: BaseValueModel) {
        if(model.value != ""){
            return;
        }
        if(model.type == 1) {
            model.value = "";
        }else if(model.type == 2) {
            model.value = "0";
        }else if(model.type == 3) {
            model.value = "0";
        }else if(model.type == 4) {
            model.value = new Date().toISOString();
        }else if(model.type == 5) {
            model.value = new Date().toISOString();
        }
    }
}