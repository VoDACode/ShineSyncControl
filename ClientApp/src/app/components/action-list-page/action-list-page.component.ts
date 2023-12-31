import { Component } from '@angular/core';
import { ShortActionModel } from 'src/app/models/short.action.model';
import { ActionApiService } from 'src/app/services/action-api.service';

@Component({
  selector: 'app-action-list-page',
  templateUrl: './action-list-page.component.html',
  styleUrls: ['./action-list-page.component.css']
})
export class ActionListPageComponent {
  public actions: ShortActionModel[] = [];

  constructor(private actionApiService: ActionApiService) {
    this.actionApiService.getActions().subscribe((response) => {
      if (response.success && response.data) {
        this.actions = response.data;
      }
    });
  }
}
