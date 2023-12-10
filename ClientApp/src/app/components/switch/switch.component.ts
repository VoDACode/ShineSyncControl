import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-switch',
  templateUrl: './switch.component.html',
  styleUrls: ['./switch.component.css']
})
export class SwitchComponent {
  @Input()
  public status: boolean = false;
  @Input()
  disabled: boolean = false;

  @Output()
  public switch: EventEmitter<boolean> = new EventEmitter<boolean>();

  toggle() {
    if (this.disabled) {
      return;
    }
    this.status = !this.status;
    this.switch.emit(this.status);
  }
}
