import { Component, OnInit } from '@angular/core';
import { Providers, Msal2Provider, TemplateHelper } from '@microsoft/mgt';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent  implements OnInit{
  title = 'demo-mgt-angular';

  ngOnInit(): void {
    Providers.globalProvider = new Msal2Provider({clientId: 'a454eacb-38d4-40bc-bab6-1d4874c7dbde'
     , scopes: ['mail.read','calendars.read', 'user.read', 'user.readbasic.all',  'profile', 'people.read', 'presence.read.all']
    });
    TemplateHelper.setBindingSyntax('[[',']]');
  }

  
}
