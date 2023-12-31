import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-save-changes-model',
  templateUrl: './save-changes-model.component.html',
  styleUrls: ['./save-changes-model.component.css']
})
export class SaveChangesModelComponent {
  @Input() public visible = false;
  @Input() public message = '';
  @Input() public yesText = this.translate.instant('button.yes');
  @Input() public noText = this.translate.instant('button.no');
  @Input() public disableYes = false;
  @Input() public disableNo = false;

  @Output() 
  public yes: EventEmitter<any> = new EventEmitter();
  @Output()
  public no: EventEmitter<any> = new EventEmitter();

  constructor(private translate: TranslateService) { }

  public onYes() {
    this.visible = false;
    this.yes.emit();
  }

  public onNo() {
    this.visible = false;
    this.no.emit();
  }
}
