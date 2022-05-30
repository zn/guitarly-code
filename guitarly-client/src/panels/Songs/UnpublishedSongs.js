import React, { useState, useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import InfiniteScroll from 'react-infinite-scroller';
import { Panel, PanelHeader, PanelHeaderButton, ScreenSpinner, Spinner, Cell, Div, SimpleCell } from '@vkontakte/vkui';

import { IOS, Placeholder, platform } from '@vkontakte/vkui';

import { useFirstPageCheck, useParams, useRouter } from '@happysanta/router';
import { PAGE_SONG } from '../../routers';

import { Icon12View, Icon16MoreVertical, Icon12ChevronOutline, Icon24Back, Icon28ChevronBack } from '@vkontakte/icons';

import { BASE_URL } from '../../config';

import { declOfNum, getCookie } from '../../utils';

const UnpublishedSongs = ({ id }) => {
	const osName = platform();
	const router = useRouter();
	const isFirstPage = useFirstPageCheck();
	const [songs, setSongs] = useState();
	const [hasMore, setHasMore] = useState(false);

	const authToken = getCookie('auth_token', osName);

	function fetchData(page) {
		const requestOptions = {
			headers: { 'Authorization': `Bearer ${authToken}`},
		};

		fetch(BASE_URL + '/songs/unpublished?page=' + page, requestOptions)
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


	return (
		<Panel id={id}>
			<PanelHeader
				left={<PanelHeaderButton onClick={() => {
					if (isFirstPage) {
						router.replacePage(PAGE_MAIN)
					} else {
						router.popPage()
					}
				}}
					style={{ backgroundColor: 'transparent' }}>
					{osName === IOS ? <Icon28ChevronBack /> : <Icon24Back />}
				</PanelHeaderButton>}
			>
				Неопубликованные песни
			</PanelHeader>
			{!songs && <ScreenSpinner />}
			{songs &&
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
								description={'дата создания: ' + new Date(song.createdAt+'Z').toLocaleString()}>
								{song.fullTitle}
							</SimpleCell>
						)}
					</InfiniteScroll>
				</Div>
				}
		</Panel>
	);
}

export default UnpublishedSongs;
