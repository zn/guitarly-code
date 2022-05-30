import React, {useState, useEffect, Fragment} from 'react';
import PropTypes from 'prop-types';

import { Panel, PanelHeader, PanelHeaderButton, ScreenSpinner, Button, Cell, Link, SimpleCell, CellButton, Div, Avatar, IconButton, HorizontalScroll, HorizontalCell, RichCell } from '@vkontakte/vkui';

import { IOS, Placeholder, platform } from '@vkontakte/vkui';

import { useFirstPageCheck, useParams, useRouter } from '@happysanta/router';
import { PAGE_SONG, PAGE_MAIN} from '../../routers';

import {Icon12View, Icon16MoreVertical, Icon12ChevronOutline, Icon24Back, Icon28ChevronBack  } from '@vkontakte/icons';

import {BASE_URL} from '../../config';

import './Artist.css';
import {declOfNum, getCookie} from '../../utils';

const Artist = ({ id }) => {
	
	const osName = platform();
	const router = useRouter();
	const isFirstPage = useFirstPageCheck();
	const { artistId } = useParams();
	const [artist, setArtist] = useState(null);
	let authToken = getCookie('auth_token', osName);

	useEffect(() => {
		const requestOptions = {
			headers: { 'Authorization': `Bearer ${authToken}` },
		};
		async function fetchData() {
			fetch(BASE_URL + '/artists/' + artistId, requestOptions)
				.then(response => response.json())
				.then(data => setArtist(data));
		}
		fetchData();
	}, []);

	return(
	<Panel id={id}>
			{!artist && <ScreenSpinner/>}
			{artist &&
				<Fragment>
					<div className="header">
						<Button mode="tertiary" className="btn-back" onClick={() => {
							if (isFirstPage) {
								router.replacePage(PAGE_MAIN)
							} else {
								router.popPage()
							}
						}}>
							{osName === IOS ? <Icon28ChevronBack/> : <Icon24Back/>}
						</Button>
						<div className="picture">
							<img src={BASE_URL + artist.pictureOriginal} alt={artist.title} className="artist-picture"/>
						</div>
						<div className="artist-info">
							{artist.title}
							<p className="small-text">{artist.viewsNumber} {declOfNum(artist.viewsNumber, ['просмотр', 'просмотра', 'просмотров'])}
							<br/>
							{artist.songs.length} {declOfNum(artist.viewsNumber, ['песня', 'песни', 'песен'])}</p>
						</div>
					</div>
					<Div>
						{artist.songs.map((song, i) =>
							<SimpleCell
								key={'song_'+song.id}
								after={<Icon12ChevronOutline />}
								onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}
								description={song.viewsNumber + ' ' + declOfNum(song.viewsNumber, ['просмотр', 'просмотра', 'просмотров'])}>
								#{i+1} {song.title}
							</SimpleCell>
						)}
					</Div>
				</Fragment>
			}
	</Panel>
);}

export default Artist;
