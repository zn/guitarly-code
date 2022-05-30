import React, {Fragment, useState, useEffect} from 'react';
import PropTypes from 'prop-types';
import { useRouter, useLocation } from '@happysanta/router';
import InfiniteScroll from 'react-infinite-scroller';

import { usePlatform, Panel, PanelHeader, ScreenSpinner, SimpleCell, Spinner, Div, Placeholder } from '@vkontakte/vkui';
import { BASE_URL } from '../../config';
import { Icon56SearchOutline } from '@vkontakte/icons';
import { PAGE_SONG, PAGE_ARTIST } from '../../routers';

import {Icon12ChevronOutline,Icon56NotebookCheckOutline } from '@vkontakte/icons';
import { declOfNum, getCookie } from '../../utils';

const SavedItems = ({ id }) => {
	
	const platform = usePlatform();
	const router = useRouter();
	const location = useLocation();
	const [songs, setSongs] = useState();
	const [hasMore, setHasMore] = useState(false);

	function fetchData(page) {
		const requestOptions = {
			headers: { 'Authorization': `Bearer ${getCookie('auth_token', platform)}`},
		};
		fetch(BASE_URL + '/songs/favorites?page=' + page, requestOptions)
			.then(response => response.json())
			.then(data => {
				setSongs(songs ? songs.concat(data) : data);
				if (data.length === 0) {
					setHasMore(false);
				} else {
					setHasMore(true);
				}
			});
	}

	useEffect(() => {
		fetchData(1);
	}, []);

	
	return(
	<Panel id={id}>
		<PanelHeader>Песенник</PanelHeader>
		{!songs && <ScreenSpinner />}
			{songs && songs.length !== 0 &&
				<Div id="scrollableDiv">
					<InfiniteScroll
						pageStart={1}
						loadMore={fetchData}
						hasMore={hasMore}
						loader={<Spinner size="small" />}
						scrollableTarget="scrollableDiv"
					>
						{songs.map((song, i) =>
							<SimpleCell
								key={'song_' + song.id}
								after={<Icon12ChevronOutline />}
								onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}
								description={song.viewsNumber + ' ' + declOfNum(song.viewsNumber, ['просмотр', 'просмотра', 'просмотров'])}>
								{song.fullTitle}
							</SimpleCell>
						)}
					</InfiniteScroll>
				</Div>
				}
			{songs && songs.length === 0 &&
				<Placeholder
					icon={<Icon56NotebookCheckOutline />}
				>
					Песенник пуст
				</Placeholder>
			}
	</Panel>

	);}

export default SavedItems;
