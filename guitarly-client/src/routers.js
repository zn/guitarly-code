import { Page, Router } from '@happysanta/router';

export const PAGE_MAIN = '/';
export const PAGE_SONG = '/songs/:songId([0-9]+)';
export const PAGE_ARTIST = '/artists/:artistId([0-9]+)';
export const PAGE_NEW_SONGS = '/songs/new';
export const PAGE_TOP_SONGS = '/songs/top';
export const PAGE_TOP_ARTISTS = '/artists/top';
export const PAGE_SEARCH = '/home/search';
export const PAGE_SAVED = '/home/saved';
export const PAGE_UNPUBLISHED_SONGS = '/songs/unpublished';

export const PAGE_EDIT_SONG = '/songs/edit/:songId([0-9]+)';
export const PAGE_ADD_SONG = '/songs/add';
export const PAGE_ADD_ARTIST = '/aritsts/add';

export const PANEL_MAIN = 'panel_main';
export const PANEL_SONG = 'panel_song';
export const PANEL_ARTIST = 'panel_artist';
export const PANEL_NEW_SONGS = 'panel_new_songs';
export const PANEL_TOP_SONGS = 'panel_top_songs';
export const PANEL_TOP_ARTISTS = 'panel_top_artists';
export const PANEL_SEARCH = 'panel_search';
export const PANEL_SAVED = 'panel_saved';
export const PANEL_UNPUBLISHED_SONGS = 'panel_unpublished_songs';

export const PANEL_EDIT_SONG = 'panel_edit_song';
export const PANEL_ADD_SONG = 'panel_add_song';
export const PANEL_ADD_ARTIST = 'panel_add_artist';

export const VIEW_MAIN = 'view_main';
export const VIEW_SEARCH = 'view_search';
export const VIEW_SAVED = 'view_saved';

export const MODAL_SHARE_MOBILE = 'modal_share_mobile';
export const MODAL_SHARE_DESKTOP = 'modal_share_desktop';
export const MODAL_TRANSPOSE = 'modal_transpose';


const routes = {
    [PAGE_MAIN]: new Page(PANEL_MAIN, VIEW_MAIN),
    [PAGE_SONG]: new Page(PANEL_SONG, VIEW_MAIN),
    [PAGE_ARTIST]: new Page(PANEL_ARTIST, VIEW_MAIN),
    [PAGE_NEW_SONGS]: new Page(PANEL_NEW_SONGS, VIEW_MAIN),
    [PAGE_TOP_SONGS]: new Page(PANEL_TOP_SONGS, VIEW_MAIN),
    [PAGE_TOP_ARTISTS]: new Page(PANEL_TOP_ARTISTS, VIEW_MAIN),
    [PAGE_SEARCH]: new Page(PANEL_SEARCH, VIEW_SEARCH),
    [PAGE_SAVED]: new Page(PANEL_SAVED, VIEW_SAVED),
    [PAGE_UNPUBLISHED_SONGS]: new Page(PANEL_UNPUBLISHED_SONGS, VIEW_MAIN),
    [PAGE_EDIT_SONG]: new Page(PANEL_EDIT_SONG, VIEW_MAIN),
    [PAGE_ADD_SONG]: new Page(PANEL_ADD_SONG, VIEW_MAIN),
    [PAGE_ADD_ARTIST]: new Page(PANEL_ADD_ARTIST, VIEW_MAIN),
  };
  
  export const router = new Router(routes);

  // router.on('update', (nextRote, oldRoute) => {
  //   nextRote.getPageId() // /product/:id([0-9]+)
  //   nextRote.getParams() // { id: "12" }
  //   nextRote.getPanelId() // panel_product
  //   nextRote.getViewId() // view_main
  //   nextRote.getLocation() // /product/12
  //   nextRote.isModal() // false
  //   nextRote.isPopup() // false
  //   nextRote.hasOverlay() // false
  
  //   if (oldRoute) {
  //     console.log(`move from page ${oldRoute.getLocation()} -> ${nextRote.getLocation()}`);
  //   } else {
  //     console.log(`enter to page ${nextRote.getLocation()}`);
  //   }
  // });

  router.start();